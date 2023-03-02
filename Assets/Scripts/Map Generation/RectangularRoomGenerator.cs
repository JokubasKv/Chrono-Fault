using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RectangularRoomGenerator : AbstractMapGenerator
{
    [SerializeField] protected RectangularRoomData roomData;


    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRoomGeneration(startPosition, roomData);

        if (clearPreviuosVisualization)
        {
            tilemapVisualizer.Clear();
            tilemapVisualizer.PaintFloorTiles(floorPositions);
        }
        WalllGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRoomGeneration(Vector2Int position, RectangularRoomData parameters)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPosition = new();

        int sizeX = Random.Range(parameters.minX, parameters.maxX);
        int sizeY = Random.Range(parameters.minY, parameters.maxY);

        var path = ProceduralGenerationAlgorithms.RectangularRoomGeneration(currentPosition, sizeX, sizeY);
        floorPosition.UnionWith(path);

        return floorPosition;
    }
}
