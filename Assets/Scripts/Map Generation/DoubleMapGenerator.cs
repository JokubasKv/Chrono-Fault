using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleMapGenerator : AbstractMapGenerator
{

    [SerializeReference] AbstractMapGenerator FutureMapGenerator;
    [SerializeReference] AbstractMapGenerator PastMapGenerator;
    [SerializeField] Seed seed;
    [SerializeField] bool createMapOnStart = false;
    [SerializeField] DualMapData dualMapData;

    public override HashSet<Vector2Int> GenerateFloor()
    {
        throw new NotImplementedException();
    }

    protected override void RunProceduralGeneration()
    {
        DoubleMapGeneration();
    }

    private void DoubleMapGeneration()
    {
        var levelsInstance = LevelsManager.instance;

        seed.SetSeed();
        var codrridorFirstPast = PastMapGenerator as CorridorFirstMapGenerator;
        codrridorFirstPast.corridorCount = levelsInstance.corridorCount;
        codrridorFirstPast.corridorLength = levelsInstance.corridorLength;
        codrridorFirstPast.GenerateDungeon();

        dualMapData.offset = new Vector2Int(0, HelperAlgorithms.GetSizeY(dualMapData.PastMapData.AllFloorTiles) + 100);


        seed.SetSeed();
        var codrridorFirstFuture = FutureMapGenerator as CorridorFirstMapGenerator;
        codrridorFirstFuture.corridorCount = levelsInstance.corridorCount;
        codrridorFirstFuture.corridorLength = levelsInstance.corridorLength;
        codrridorFirstFuture.startPosition += dualMapData.offset;
        codrridorFirstFuture.GenerateDungeon();
        codrridorFirstFuture.startPosition -= dualMapData.offset;
    }

    public override void Clear()
    {
        tilemapVisualizer.Clear();
        PastMapGenerator.Clear();
        FutureMapGenerator.Clear();
    }

    private void Start()
    {
        if (createMapOnStart)
        {
            seed.GenerateGameSeed();
            Clear();
            RunProceduralGeneration();
        }
    }
}
