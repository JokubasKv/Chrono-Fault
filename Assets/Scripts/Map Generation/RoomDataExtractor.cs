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
}

