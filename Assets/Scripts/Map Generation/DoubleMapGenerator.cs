using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleMapGenerator : AbstractMapGenerator
{
    [SerializeField] private Vector2Int offSet = new Vector2Int(0,360);

    [SerializeReference] AbstractMapGenerator FutureMapGenerator;
    [SerializeReference] AbstractMapGenerator PastMapGenerator;
    [SerializeField] Seed seed;
    [SerializeField] bool createMapOnStart = false;

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
        seed.SetSeed();
        PastMapGenerator.GenerateDungeon();

        seed.SetSeed();
        FutureMapGenerator.startPosition += offSet;
        FutureMapGenerator.GenerateDungeon();
        FutureMapGenerator.startPosition -= offSet;
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
