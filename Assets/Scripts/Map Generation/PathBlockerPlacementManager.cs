using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PathBlockerPlacementManager : MonoBehaviour
{
    [SerializeField] MapData pastMapData;
    [SerializeField] MapData futureMapData;

    [SerializeField]
    private List<PropData> blockerToPlace;

    [SerializeField, Range(0, 1)]
    private float blockerPlacementChance = 0.7f;

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinished;



    public void ProccesBlockers()
    {
        if (pastMapData == null || futureMapData == null)
        {
            Debug.Log("Missing Map Data For Blockers Generation");
            return;
        }
        Debug.Log("Doh");
        for (int i = 0; i < pastMapData.Paths.Count; i++)
        {
            if (UnityEngine.Random.value < blockerPlacementChance)
            {
                PropData blocker = blockerToPlace[Random.Range(0, blockerToPlace.Count)];

                switch (UnityEngine.Random.Range(0, 2))
                {
                    case 0:
                        PlaceBlockers(pastMapData.Paths[i], blocker, pastMapData);
                        break;
                    case 1:
                        PlaceBlockers(futureMapData.Paths[i], blocker, futureMapData);
                        break;
                }
            }
        }

        //OnFinished?.Invoke();
        Invoke("RunEvent", 1);

    }  

    private void PlaceBlockers(Path path, PropData blocker, MapData mapData)
    {
        Vector2Int middlePoint = (path.StartPos + path.EndPos) / 2;
        List<Vector2Int> perpandicularDirections = Direction2d.GetPerpandicular(path.Direction);

        foreach (var direction in perpandicularDirections)
        {
            Vector2Int currentposition = middlePoint;
            while (true)
            {
                if (!path.FloorTiles.Contains(currentposition))
                    break;
                PlacePropGameObjectAt(path, currentposition, blocker);
                currentposition += direction;
            }
        }
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
    /// Place a PropData as a new GameObject at a specified position
    /// </summary>
    /// <param name="room"></param>
    /// <param name="placementPostion"></param>
    /// <param name="propToPlace"></param>
    /// <returns></returns>
    private GameObject PlacePropGameObjectAt(Path path, Vector2Int placementPostion, PropData propToPlace)
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
        path.BlockerPositions.Add(placementPostion);
        path.BlockerObjectReferences.Add(PropData);
        return PropData;
    }
}

/// <summary>
/// Where to start placing the PropData ex. start at BottomLeft corner and search 
/// if there are free space to the Right and Up in case of placing a biggex PropData
/// </summary>
public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}
