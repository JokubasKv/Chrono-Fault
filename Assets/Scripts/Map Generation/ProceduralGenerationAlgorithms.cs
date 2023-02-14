using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class ProceduralGenerationAlgorithms 
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new();

        path.Add(startPosition);
        var previousposition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousposition + Direction2d.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousposition = newPosition;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new();

        var direction = Direction2d.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(startPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new();
        List<BoundsInt> roomsList = new();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }

        if (roomsList.Count == 0)
        {
            roomsQueue.Enqueue(spaceToSplit);
            roomsList.Add(roomsQueue.Dequeue());
            if (roomsList.Count == 0) Debug.LogError("0 ROOM GENERATE");
            else Debug.Log("room generated -> " + roomsList.Count);
        }

        return roomsList;
    }

    private static void SplitVertically(int minWidth,  Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally( int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2d
{
    public static List<Vector2Int> cardinalDirections = new()
    {
        new Vector2Int(0,1),  //Up
        new Vector2Int(1,0),  //Right
        new Vector2Int(0,-1), //Down
        new Vector2Int(-1,0), //Left
    };

    public static List<Vector2Int> diagonalDirections = new List<Vector2Int>
    {
        new Vector2Int(1,1),     //UP-RIGHT
        new Vector2Int(1,-1),    //RIGHT-DOWN
        new Vector2Int(-1, -1),  //DOWN-LEFT
        new Vector2Int(-1, 1)    //LEFT-UP
    };

    public static List<Vector2Int> eightDirections = new List<Vector2Int>
    {
        new Vector2Int(0,1),    //UP
        new Vector2Int(1,1),    //UP-RIGHT
        new Vector2Int(1,0),    //RIGHT
        new Vector2Int(1,-1),   //RIGHT-DOWN
        new Vector2Int(0, -1),  // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0),  //LEFT
        new Vector2Int(-1, 1)   //LEFT-UP

    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirections[Random.Range(0, cardinalDirections.Count)];
    }
}