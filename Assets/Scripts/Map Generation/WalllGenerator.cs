using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WalllGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPosition = FindWallsInDirections(floorPositions, Direction2d.cardinalDirections);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2d.diagonalDirections);
        CreateBasicWalls(tilemapVisualizer, basicWallPosition, floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinary = "";
            foreach (var direction in Direction2d.eightDirections)
            {
                var neighbourPositions = position + direction;
                if (floorPositions.Contains(neighbourPositions))
                {
                    neighboursBinary += "1";
                }
                else
                {
                    neighboursBinary += "0";
                }
            }
            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinary);
        }
    }

    private static void CreateBasicWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPosition, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPosition)
        {
            string neighboursBinary = "";
            foreach (var direction in Direction2d.cardinalDirections)
            {
                var neighbourPositions = position + direction;
                if (floorPositions.Contains(neighbourPositions))
                {
                    neighboursBinary += "1";
                }
                else
                {
                    neighboursBinary += "0";
                }
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinary);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> cardinalDirections)
    {
        HashSet<Vector2Int> wallpositions = new();

        foreach(var position in floorPositions)
        {
            foreach (var direction in cardinalDirections)
            {
                var neighbourPosition = position + direction;
                if(floorPositions.Contains(neighbourPosition) == false)
                {
                    wallpositions.Add(neighbourPosition);
                }
            }
        }

        return wallpositions;
    }
}
