using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class RoomDataExtractor : MonoBehaviour
{
    [SerializeField] private MapData dungeonData;

    [SerializeField] private bool showGizmo = false;

    public UnityEvent OnFinishedRoomProcessing;

    Dictionary<Vector2Int, int> weightedTiles = new Dictionary<Vector2Int, int>(); 
    private void Awake()
    {

    }
    public void ProcessRooms()
    {
        if (dungeonData == null)
            return;

        foreach (Room room in dungeonData.Rooms)
        {
            //find corener, near wall and inner tiles
            foreach (Vector2Int tilePosition in room.FloorTiles)
            {
                int neighboursCount = 4;

                if (room.FloorTiles.Contains(tilePosition + Vector2Int.up) == false)
                {
                    room.NearWallTilesUp.Add(tilePosition);
                    neighboursCount--;
                }
                if (room.FloorTiles.Contains(tilePosition + Vector2Int.down) == false)
                {
                    room.NearWallTilesDown.Add(tilePosition);
                    neighboursCount--;
                }
                if (room.FloorTiles.Contains(tilePosition + Vector2Int.right) == false)
                {
                    room.NearWallTilesRight.Add(tilePosition);
                    neighboursCount--;
                }
                if (room.FloorTiles.Contains(tilePosition + Vector2Int.left) == false)
                {
                    room.NearWallTilesLeft.Add(tilePosition);
                    neighboursCount--;
                }

                //find corners
                if (neighboursCount <= 2)
                    room.CornerTiles.Add(tilePosition);

                if (neighboursCount == 4)
                    room.InnerTiles.Add(tilePosition);
            }

            room.NearWallTilesUp.ExceptWith(room.CornerTiles);
            room.NearWallTilesDown.ExceptWith(room.CornerTiles);
            room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
            room.NearWallTilesRight.ExceptWith(room.CornerTiles);
        }

        SetupRoomTypes();


        OnFinishedRoomProcessing?.Invoke();
    }

    private void SetupRoomTypes()
    {
        

        dungeonData.CombineAllFloorTiles();

        TileGraph tileGraph = new TileGraph(dungeonData.AllFloorTiles);
        weightedTiles = tileGraph.GetWeightedBFS(dungeonData.Rooms[0].RoomCenterPos, new HashSet<Vector2Int>());

        KeyValuePair<Vector2Int, int> result = weightedTiles
            .Where(entry => dungeonData.Rooms.Any(room => room.RoomCenterPos == entry.Key))
            .OrderByDescending(entry => entry.Value)
            .FirstOrDefault();
        //Debug.Log(result.Value);

        dungeonData.Rooms
            .Where(room => room.RoomCenterPos == result.Key)
            .FirstOrDefault()
            .RoomType = RoomTypes.Boss;
        dungeonData.Rooms[0].RoomType = RoomTypes.Starting;

        bool unnasigned = true;
        while (unnasigned)
        {
            int randomIndex = Random.Range(0, dungeonData.Rooms.Count);
            if (dungeonData.Rooms[randomIndex].RoomType == RoomTypes.Normal)
            {
                dungeonData.Rooms[randomIndex].RoomType = RoomTypes.Item;
                unnasigned = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (dungeonData == null || showGizmo == false)
            return;
        /*foreach (Room room in dungeonData.Rooms)
        {
            //Draw inner tiles
            Gizmos.color = Color.yellow;
            foreach (Vector2Int floorPosition in room.InnerTiles)
            {
                if (dungeonData.Paths.All(floors => floors.FloorTiles.Contains(floorPosition)))
                {
                    Debug.Log(floorPosition);
                    continue;
                }
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles UP
            Gizmos.color = Color.blue;
            foreach (Vector2Int floorPosition in room.NearWallTilesUp)
            {
                if (dungeonData.Paths.All(floors => floors.FloorTiles.Contains(floorPosition)))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles DOWN
            Gizmos.color = Color.green;
            foreach (Vector2Int floorPosition in room.NearWallTilesDown)
            {
                if (dungeonData.Paths.All(floors => floors.FloorTiles.Contains(floorPosition)))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles RIGHT
            Gizmos.color = Color.white;
            foreach (Vector2Int floorPosition in room.NearWallTilesRight)
            {
                if (dungeonData.Paths.All(floors => floors.FloorTiles.Contains(floorPosition)))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles LEFT
            Gizmos.color = Color.cyan;
            foreach (Vector2Int floorPosition in room.NearWallTilesLeft)
            {
                if (dungeonData.Paths.All(floors => floors.FloorTiles.Contains(floorPosition)))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles CORNERS
            Gizmos.color = Color.magenta;
            foreach (Vector2Int floorPosition in room.CornerTiles)
            {
                if (dungeonData.Paths.All(floors => floors.FloorTiles.Contains(floorPosition)))
                    continue;
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
        }
        foreach (Path path in dungeonData.Paths)
        {
            Gizmos.color = Color.black;
            foreach (Vector2Int floorPosition in path.FloorTiles)
            {
                Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
        }*/
        /*foreach (var item in weightedTiles)
        {
            Handles.Label(new Vector3(item.Key.x, item.Key.y, 0), item.Value.ToString());
        }*/
        
    }
}

