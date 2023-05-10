using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class CorridorFirstMapGenerator : AbstractMapGenerator
{

    [SerializeField] public int corridorLength = 14;
    [SerializeField] public int corridorWidth = 2;
    [SerializeField] public int corridorCount = 5;

    [SerializeField] [Range(0f,1f)] public float roomPercent = 0.6f;

    [SerializeReference] public AbstractMapGenerator roomGenerator;

    [SerializeField] private MapData mapData;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstMapGeneration();
    }

    private void CorridorFirstMapGeneration()
    {
        HashSet<Vector2Int> floorPositions = new();
        HashSet<Vector2Int> potentialRoomPositions = new();

        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WalllGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        OnFinishedRoomGeneration.Invoke();
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if(roomFloors.Contains(position) == false)
            {
                roomGenerator.startPosition = position;
                var room = roomGenerator.GenerateFloor();

                SaveRoomData(position, room);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new();
        foreach (var position in floorPositions)
        {
            int neighbourCount = 0;
            foreach (var direction in Direction2d.cardinalDirections)
            {
                if (floorPositions.Contains(position + direction))
                {
                    neighbourCount++;
                }
            }
            if(neighbourCount == 1)
            {
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new();
        var roomsToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions
            .OrderBy(x => Random.Range(0,6))
            .Take(roomsToCreateCount)
            .ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            roomGenerator.startPosition = roomPosition;
            var roomFloor = roomGenerator.GenerateFloor();

            SaveRoomData(roomPosition, roomFloor);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    public void ClearRoomData()
    {
        mapData.Reset();
    }

    private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    {
        mapData.Rooms.Add(new Room(roomPosition, roomFloor));
    }
    private void SavePathData(Vector2Int startPosition, Vector2Int endPosition, Vector2Int direction, HashSet<Vector2Int> roomFloor)
    {
        mapData.Paths.Add(new Path(roomFloor, startPosition, endPosition, direction));
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        for (int i = 0; i < corridorCount; i++)
        {
            Vector2Int direction;
            var path = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength, corridorWidth, out direction);
            SavePathData(currentPosition, path[path.Count - 1], direction, new HashSet<Vector2Int>(path));
            currentPosition = path[path.Count - 1];
            floorPositions.UnionWith(path);
            potentialRoomPositions.Add(currentPosition);
        }
    }

    public override HashSet<Vector2Int> GenerateFloor()
    {
        throw new NotImplementedException();
    }

    public override void Clear()
    {
        tilemapVisualizer.Clear();
        ClearRoomData();
    }
}
