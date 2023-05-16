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
            if (Random.value < blockerPlacementChance)
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

        OnFinished?.Invoke();

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

    private GameObject PlacePropGameObjectAt(Path path, Vector2Int placementPostion, PropData propToPlace)
    {
        if (propToPlace.PropPrefab != null)
        {
            GameObject gameObject = Instantiate(propToPlace.PropPrefab);

            gameObject.transform.localPosition = (Vector2)placementPostion + Vector2.one * 0.5f;

            path.BlockerPositions.Add(placementPostion);
            path.BlockerObjectReferences.Add(gameObject);

            return gameObject;
        }
        else
        {
            Debug.Log("Missing Gameobject on " + propToPlace);
            return null;
        }
    }
}

