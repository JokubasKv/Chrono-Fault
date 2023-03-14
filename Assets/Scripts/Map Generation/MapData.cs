using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all the data about our dungeon.
/// Useful when creating a save / load system
/// </summary>
public class MapData : MonoBehaviour
{
    public List<Room> Rooms { get; set; } = new List<Room>();
    public List<Path> Paths { get; set; } = new List<Path>();

    public GameObject PlayerReference { get; set; }
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
        DestroyImmediate(PlayerReference);
    }

    public IEnumerator TutorialCoroutine(Action code)
    {
        yield return new WaitForSeconds(1);
        code();
    }
}


/// <summary>
/// Holds all the data about the room
/// </summary>
public class Room
{
    public Vector2 RoomCenterPos { get; set; }
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

    public Room(Vector2 roomCenterPos, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
    }
}

/// <summary>
/// Holds all the data about the connecting paths
/// </summary>
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