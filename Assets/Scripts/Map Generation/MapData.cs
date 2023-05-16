using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public List<Room> Rooms { get; set; } = new List<Room>();
    public List<Path> Paths { get; set; } = new List<Path>();

    public HashSet<Vector2Int> AllFloorTiles { get; set; } = new HashSet<Vector2Int>();

    public void CombineAllFloorTiles()
    {
        AllFloorTiles.Clear();
        foreach (Room room in Rooms)
        {
            AllFloorTiles.UnionWith(room.FloorTiles);
        }
        foreach (Path path in Paths)
        {
            AllFloorTiles.UnionWith(path.FloorTiles);
        }
    }

    public void Reset()
    {
        foreach (Room room in Rooms)
        {
            foreach (var item in room.PropObjectReferences)
            {
                DestroyImmediate(item);
            }
            foreach (var item in room.EnemiesInTheRoom)
            {
                DestroyImmediate(item);
            }
        }
        foreach (Path path in Paths)
        {
            foreach (var item in path.BlockerObjectReferences)
            {
                DestroyImmediate(item);
            }
        }
        Rooms = new();
        Paths = new();
    }
}


public class Room
{
    public RoomTypes RoomType { get; set; } = RoomTypes.Normal;
    public Vector2Int RoomCenterPos { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> NearWallTilesUp { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesDown { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesLeft { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesRight { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> PropPositions { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> PropObjectReferences { get; set; } = new List<GameObject>();

    public List<Vector2Int> PositionsAccessibleFromPath { get; set; } = new List<Vector2Int>();

    public List<GameObject> EnemiesInTheRoom { get; set; } = new List<GameObject>();

    public Room(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
    }
}

public class Path
{
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();

    public Vector2Int StartPos { get; set; }
    public Vector2Int EndPos { get; set; }
    public Vector2Int Direction { get; set; }

    public HashSet<Vector2Int> BlockerPositions { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> BlockerObjectReferences { get; set; } = new List<GameObject>();

    public Path(HashSet<Vector2Int> floorTiles, Vector2Int startPos, Vector2Int endPos, Vector2Int direction)
    {
        FloorTiles = floorTiles;
        StartPos = startPos;
        EndPos = endPos;
        Direction = direction;
    }
}

public enum RoomTypes
{
    Starting,
    Normal,
    Item,
    Boss
}