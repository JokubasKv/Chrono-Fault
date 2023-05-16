using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PropPlacementManager : MonoBehaviour
{
    [SerializeField]
    MapData dungeonData;

    [SerializeField]
    private List<PropData> propsToPlace;

    [SerializeField]
    private List<PropData> startingRoomPropsToPlace;

    [SerializeField]
    private List<GameObject> itemsToPlace;

    [SerializeField]
    private GameObject LeavePrefab;

    [SerializeField, Range(0, 1)]
    private float cornerPropPlacementChance = 0.7f;

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinished;

    private void Awake()
    {
        if (dungeonData == null)
            dungeonData = gameObject.GetComponent<MapData>();
    }

    public void ProcessRooms()
    {
        if (dungeonData == null)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            switch (room.RoomType)
            {
                case RoomTypes.Normal:
                    PlaceProps(room, propsToPlace);
                    break;

                case RoomTypes.Starting:
                    PlaceProps(room, startingRoomPropsToPlace);
                    break;

                case RoomTypes.Item:
                    PlaceGameObject(room, itemsToPlace[Random.Range(0, itemsToPlace.Count())], room.InnerTiles);
                    break;

                case RoomTypes.Boss:
                    PlaceGameObject(room, LeavePrefab, room.InnerTiles);
                    break;
            }
        }

        OnFinished?.Invoke();
    }

    private void PlaceProps(Room room, List<PropData> propList)
    {
        List<PropData> cornerProps = propList
            .Where(x => x.Corner)
            .ToList();
        PlaceCornerProps(room, cornerProps);

        List<PropData> leftWallProps = propList
        .Where(x => x.NearWallLeft)
        .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
        .ToList();
        PlaceProps(room, leftWallProps, room.NearWallTilesLeft);

        List<PropData> rightWallProps = propList
        .Where(x => x.NearWallRight)
        .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
        .ToList();
        PlaceProps(room, rightWallProps, room.NearWallTilesRight);


        List<PropData> topWallProps = propList
        .Where(x => x.NearWallUP)
        .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
        .ToList();
        PlaceProps(room, topWallProps, room.NearWallTilesUp);

        List<PropData> downWallProps = propList
        .Where(x => x.NearWallDown)
        .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
        .ToList();
        PlaceProps(room, downWallProps, room.NearWallTilesDown);

        List<PropData> innerProps = propList
            .Where(x => x.Inner)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
        PlaceProps(room, innerProps, room.InnerTiles);
    }

    private void PlaceGameObject(
    Room room, GameObject gameObject, HashSet<Vector2Int> availableTiles)
    {
        GameObject go = Instantiate(gameObject);
        var availableTile = availableTiles.ElementAt(Random.Range(0, availableTiles.Count));
        go.transform.localPosition = (Vector2)availableTile + Vector2.one * 0.5f;
        room.PropPositions.Add(availableTile);
    }

    private void PlaceProps(Room room, List<PropData> wallProps, HashSet<Vector2Int> availableTiles)
    {

        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        foreach (var path in dungeonData.Paths)
        {
            tempPositons.ExceptWith(path.FloorTiles);
        }


        foreach (PropData propToPlace in wallProps)
        {
            int quantity = Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                tempPositons.ExceptWith(room.PropPositions);
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                if (TryPlacingPropBruteForce(room, propToPlace, availablePositions) == false)
                    break;
            }

        }
    }

    private bool TryPlacingPropBruteForce(Room room, PropData propToPlace, List<Vector2Int> availablePositions)
    {
        for (int i = 0; i < availablePositions.Count; i++)
        {
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;


            List<Vector2Int> freePositionsAround
                = TryToFitProp(propToPlace, availablePositions, position);


            if (freePositionsAround.Count == propToPlace.PropSize.x * propToPlace.PropSize.y)
            {

                PlacePropGameObjectAt(room, position, propToPlace);

                foreach (Vector2Int pos in freePositionsAround)
                {
                    room.PropPositions.Add(pos);
                }

                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, propToPlace, 1);
                }
                return true;
            }
        }

        return false;
    }

    private List<Vector2Int> TryToFitProp(PropData PropData, List<Vector2Int> availablePositions, Vector2Int originPosition)
    {
        List<Vector2Int> freePositions = new();

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

        return freePositions;
    }

    private void PlaceCornerProps(Room room, List<PropData> cornerProps)
    {
        if (cornerProps.Count == 0)
            return;
        float tempChance = cornerPropPlacementChance;

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (Random.value < tempChance)
            {
                PropData propToPlace
                    = cornerProps[Random.Range(0, cornerProps.Count)];

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

    private void PlaceGroupObject(
        Room room, Vector2Int groupOriginPosition, PropData propToPlace, int searchOffset)
    {
        int count = Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);


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

        availableSpaces.OrderBy(x => Guid.NewGuid());

        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropGameObjectAt(room, availableSpaces[i], propToPlace);
        }

    }


    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPostion, PropData propToPlace)
    {
        if (propToPlace.PropPrefab != null)
        {
            GameObject gameObject = Instantiate(propToPlace.PropPrefab);

            gameObject.transform.localPosition = (Vector2)placementPostion + Vector2.one * 0.5f;

            room.PropPositions.Add(placementPostion);
            room.PropObjectReferences.Add(gameObject);

            return gameObject;
        }
        else
        {
            Debug.Log("Missing Gameobject on " + propToPlace);
            return null;
        }
    }
}
