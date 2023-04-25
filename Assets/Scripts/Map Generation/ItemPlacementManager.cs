using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ItemPlacementManager : MonoBehaviour
{
    [SerializeField] MapData pastMapData;
    [SerializeField] MapData futureMapData;

    [SerializeField]
    private List<GameObject> itemsToPlace;

    [SerializeField, Range(0, 1)]
    private float blockerPlacementChance = 0.7f;

    public UnityEvent OnFinished;



    public void ProccesBlockers()
    {
        if (pastMapData == null || futureMapData == null)
        {
            Debug.Log("Missing Map Data For Blockers Generation");
            return;
        }

        CreateItems(pastMapData);
        CreateItems(futureMapData);

        //OnFinished?.Invoke();
        Invoke("RunEvent", 1);

    }

    private void CreateItems(MapData mapdata)
    {
        var pastCombinedEverything = new HashSet<Vector2Int>();
        for (int i = 0; i < mapdata.Paths.Count; i++)
        {
            pastCombinedEverything.UnionWith(mapdata.Paths[i].FloorTiles);
        }

        for (int i = 0; i < mapdata.Rooms.Count; i++)
        {
            pastCombinedEverything.UnionWith(mapdata.Rooms[i].FloorTiles);
        }

        RoomGraph roomGraph = new RoomGraph(pastCombinedEverything);
        var stuff = roomGraph.RunBFS(mapdata.Rooms[0].FloorTiles.First(), new HashSet<Vector2Int>());

        for (int i = 0; i < mapdata.Rooms.Count; i++)
        {
            if (UnityEngine.Random.value < blockerPlacementChance)
            {
                Vector2Int randomTile = mapdata.Rooms[i].FloorTiles.ElementAt(Random.Range(0, mapdata.Rooms.Count));
                Debug.Log(roomGraph.graphPositionWeight[randomTile] + " |" + randomTile);
                PlacePropGameObjectAt(randomTile, itemsToPlace[Random.Range(0, itemsToPlace.Count)]);
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
    private void PlacePropGameObjectAt(Vector2Int placementPostion, GameObject propToPlace)
    {
        GameObject enemy = Instantiate(propToPlace);
        enemy.transform.localPosition = (Vector2)placementPostion + Vector2.one * 0.5f;
    }
}