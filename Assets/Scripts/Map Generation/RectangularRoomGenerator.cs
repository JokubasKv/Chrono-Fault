using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RectangularRoomGenerator : AbstractMapGenerator
{
    [SerializeField] protected List<RectangularRoomDataWeighted> roomData;

    [Serializable]
    public struct RectangularRoomDataWeighted
    {
        public RectangularRoomData data;
        public int weight;
    }


    public override HashSet<Vector2Int> GenerateFloor()
    {
        int index = HelperAlgorithms.GetRandomWeightedIndex(roomData.Select(x => x.weight).ToArray());
        return RunRoomGeneration(startPosition, roomData[index].data);
    }

    protected override void RunProceduralGeneration()
    {
        int index = HelperAlgorithms.GetRandomWeightedIndex(roomData.Select(x => x.weight).ToArray());
        HashSet<Vector2Int> floorPositions = RunRoomGeneration(startPosition, roomData[index].data);

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
    public override void Clear()
    {
        tilemapVisualizer.Clear();
    }
}
