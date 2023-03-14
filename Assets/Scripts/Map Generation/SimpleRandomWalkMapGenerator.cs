using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkMapGenerator : AbstractMapGenerator
{
    [SerializeField] protected List<RandomWalkDataWeighted> randomWalkData;

    [Serializable]
    public struct RandomWalkDataWeighted
    {
        public SimpleRandomWalkData data;
        public int weight;
    }


    public override HashSet<Vector2Int> GenerateFloor()
    {
        int index = HelperAlgorithms.GetRandomWeightedIndex(randomWalkData.Select(x => x.weight).ToArray());
        return RunRandomWalk(startPosition, randomWalkData[index].data);
    }

    protected override void RunProceduralGeneration()
    {
        int[] weightsArray = randomWalkData.Select(x => x.weight).ToArray();
        int index = HelperAlgorithms.GetRandomWeightedIndex(weightsArray);
        Debug.Log(index);
        HashSet<Vector2Int> floorPositions = RunRandomWalk(startPosition, randomWalkData[index].data);

        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WalllGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(Vector2Int position,SimpleRandomWalkData parameters)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPosition = new();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walklength);
            floorPosition.UnionWith(path);
            if (parameters.startRandomEachIteraion)
            {
                currentPosition = floorPosition.ElementAt(Random.Range(0, floorPosition.Count));
            }
        }
        return floorPosition;
    }
    public override void Clear()
    {
        tilemapVisualizer.Clear();
    }
}
