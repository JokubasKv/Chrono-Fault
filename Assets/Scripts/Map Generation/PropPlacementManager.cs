using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PropPlacementManager : MonoBehaviour
{
    [SerializeField]
    MapData dungeonData;

    [SerializeField]
    private List<PropData> propsToPlace;

    [SerializeField, Range(0, 1)]
    private float cornerPropPlacementChance = 0.7f;

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinished;

    private void Awake()
    {
        if(dungeonData == null)
            dungeonData = gameObject.GetComponent<MapData>();
    }

    public void ProcessRooms()
    {
        if (dungeonData == null)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            //Place props place props in the corners
            List<PropData> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
            PlaceCornerProps(room, cornerProps);

            //Place props near LEFT wall
            List<PropData> leftWallProps = propsToPlace
            .Where(x => x.NearWallLeft)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            PlaceProps(room, leftWallProps, room.NearWallTilesLeft, PlacementOriginCorner.BottomLeft);

            //Place props near RIGHT wall
            List<PropData> rightWallProps = propsToPlace
            .Where(x => x.NearWallRight)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();

            PlaceProps(room, rightWallProps, room.NearWallTilesRight, PlacementOriginCorner.TopRight);

            //Place props near UP wall
            List<PropData> topWallProps = propsToPlace
            .Where(x => x.NearWallUP)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();

            PlaceProps(room, topWallProps, room.NearWallTilesUp, PlacementOriginCorner.TopLeft);

            //Place props near DOWN wall
            List<PropData> downWallProps = propsToPlace
            .Where(x => x.NearWallDown)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();

            PlaceProps(room, downWallProps, room.NearWallTilesDown, PlacementOriginCorner.BottomLeft);

            //Place inner props
            List<PropData> innerProps = propsToPlace
                .Where(x => x.Inner)
                .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
                .ToList();
            PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
        }

        //OnFinished?.Invoke();
        Invoke("RunEvent", 1);

    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }

    private IEnumerator TutorialCoroutine(Action code)
    {
        yield return new WaitForSeconds(3);
        code();
    }

    /// <summary>
    /// Places props near walls. We need to specify the props anw the placement start point
    /// </summary>
    /// <param name="room"></param>
    /// <param name="wallProps">Props that we should try to place</param>
    /// <param name="availableTiles">Tiles that are near the specific wall</param>
    /// <param name="placement">How to place bigger props. Ex near top wall we want to start placemt from the Top corner and find if there are free spaces below</param>
    private void PlaceProps(
        Room room, List<PropData> wallProps, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {

        //Remove path positions from the initial nearWallTiles to ensure the clear path to traverse dungeon
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        foreach (var path in dungeonData.Paths)
        {
            tempPositons.ExceptWith(path.FloorTiles);
        }

        //We will try to place all the props
        foreach (PropData propToPlace in wallProps)
        {
            //We want to place only certain quantity of each PropData
            int quantity
                = UnityEngine.Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                //remove taken positions
                tempPositons.ExceptWith(room.PropPositions);
                //shuffel the positions
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                //If placement has failed there is no point in trying to place the same PropData again
                if (TryPlacingPropBruteForce(room, propToPlace, availablePositions, placement) == false)
                    break;
            }

        }
    }

    /// <summary>
    /// Tries to place the PropData using brute force (trying each available tile position)
    /// </summary>
    /// <param name="room"></param>
    /// <param name="propToPlace"></param>
    /// <param name="availablePositions"></param>
    /// <param name="placement"></param>
    /// <returns>False if there is no space. True if placement was successful</returns>
    private bool TryPlacingPropBruteForce(
        Room room, PropData propToPlace, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        //try placing the objects starting from the corner specified by the placement parameter
        for (int i = 0; i < availablePositions.Count; i++)
        {
            //select the specified position (but it can be already taken after placing the corner props as a group)
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            //check if there is enough space around to fit the PropData
            List<Vector2Int> freePositionsAround
                = TryToFitProp(propToPlace, availablePositions, position, placement);

            //If we have enough spaces place the PropData
            if (freePositionsAround.Count == propToPlace.PropSize.x * propToPlace.PropSize.y)
            {
                //Place the gameobject
                PlacePropGameObjectAt(room, position, propToPlace);
                //Lock all the positions recquired by the PropData (based on its size)
                foreach (Vector2Int pos in freePositionsAround)
                {
                    //Hashest will ignore duplicate positions
                    room.PropPositions.Add(pos);
                }

                //Deal with groups
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, propToPlace, 1);
                }
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the PropData will fit (accordig to it size)
    /// </summary>
    /// <param name="PropData"></param>
    /// <param name="availablePositions"></param>
    /// <param name="originPosition"></param>
    /// <param name="placement"></param>
    /// <returns></returns>
    private List<Vector2Int> TryToFitProp(
        PropData PropData,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition,
        PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        //Perform the specific loop depending on the PlacementOriginCorner
        if (placement == PlacementOriginCorner.BottomLeft)
        {
            for (int xOffset = 0; xOffset < PropData.PropSize.x; xOffset++)
            {
                for (int yOffset = 0; yOffset < PropData.PropSize.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.BottomRight)
        {
            for (int xOffset = -PropData.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = 0; yOffset < PropData.PropSize.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.TopLeft)
        {
            for (int xOffset = 0; xOffset < PropData.PropSize.x; xOffset++)
            {
                for (int yOffset = -PropData.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else
        {
            for (int xOffset = -PropData.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = -PropData.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }

        return freePositions;
    }

    /// <summary>
    /// Places props in the corners of the room
    /// </summary>
    /// <param name="room"></param>
    /// <param name="cornerProps"></param>
    private void PlaceCornerProps(Room room, List<PropData> cornerProps)
    {
        if (cornerProps.Count == 0)
            return;
        float tempChance = cornerPropPlacementChance;

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (UnityEngine.Random.value < tempChance)
            {
                PropData propToPlace
                    = cornerProps[UnityEngine.Random.Range(0, cornerProps.Count)];

                PlacePropGameObjectAt(room, cornerTile, propToPlace);
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, propToPlace, 2);
                }
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }

    /// <summary>
    /// Helps to find free spaces around the groupOriginPosition to place a PropData as a group
    /// </summary>
    /// <param name="room"></param>
    /// <param name="groupOriginPosition"></param>
    /// <param name="propToPlace"></param>
    /// <param name="searchOffset">The search offset ex 1 = we will check all tiles withing the distance of 1 unity away from origin position</param>
    private void PlaceGroupObject(
        Room room, Vector2Int groupOriginPosition, PropData propToPlace, int searchOffset)
    {
        //*Can work poorely when placing bigger props as groups

        //calculate how many elements are in the group -1 that we have placed in the center
        int count = UnityEngine.Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);

        //find the available spaces around the center point.
        //we use searchOffset to limit the distance between those points and the center point
        List<Vector2Int> availableSpaces = new List<Vector2Int>();
        for (int xOffset = -searchOffset; xOffset <= searchOffset; xOffset++)
        {
            for (int yOffset = -searchOffset; yOffset <= searchOffset; yOffset++)
            {
                Vector2Int tempPos = groupOriginPosition + new Vector2Int(xOffset, yOffset);
                if (room.FloorTiles.Contains(tempPos) &&
                    !dungeonData.Paths.All(floors => floors.FloorTiles.Contains(tempPos)) &&
                    !room.PropPositions.Contains(tempPos))
                {
                    availableSpaces.Add(tempPos);
                }
            }
        }

        //shuffle the list
        availableSpaces.OrderBy(x => Guid.NewGuid());

        //place the props (as many as we want or if there is less space fill all the available spaces)
        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropGameObjectAt(room, availableSpaces[i], propToPlace);
        }

    }

    /// <summary>
    /// Place a PropData as a new GameObject at a specified position
    /// </summary>
    /// <param name="room"></param>
    /// <param name="placementPostion"></param>
    /// <param name="propToPlace"></param>
    /// <returns></returns>
    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPostion, PropData propToPlace)
    {
        //Instantiat the PropData at this positon
        GameObject PropData = Instantiate(propPrefab);
        SpriteRenderer propSpriteRenderer = PropData.GetComponentInChildren<SpriteRenderer>();

        //set the sprite
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        //Add a collider
        CapsuleCollider2D collider
            = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
        collider.offset = Vector2.zero;
        if (propToPlace.PropSize.x > propToPlace.PropSize.y)
        {
            collider.direction = CapsuleDirection2D.Horizontal;
        }
        Vector2 size
            = new Vector2(propToPlace.PropSize.x * 0.8f, propToPlace.PropSize.y * 0.8f);
        collider.size = size;

        PropData.transform.localPosition = (Vector2)placementPostion;
        //adjust the position to the sprite
        propSpriteRenderer.transform.localPosition
            = (Vector2)propToPlace.PropSize * 0.5f;

        //Save the PropData in the room data (so in the dunbgeon data)
        room.PropPositions.Add(placementPostion);
        room.PropObjectReferences.Add(PropData);
        return PropData;
    }
}
