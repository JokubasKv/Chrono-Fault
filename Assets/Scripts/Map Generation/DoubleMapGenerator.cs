using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleMapGenerator : AbstractMapGenerator
{
    [SerializeField] private Vector2Int offSet = new Vector2Int(0,360);

    [SerializeReference] AbstractMapGenerator FutureMapGenerator;
    [SerializeReference] AbstractMapGenerator PresentMapGenerator;
    [SerializeField] Seed seed;

    protected override void RunProceduralGeneration()
    {
        DoubleMapGeneration();
    }

    private void DoubleMapGeneration()
    {
        seed.SetSeed();
        PresentMapGenerator.GenerateDungeon();

        seed.SetSeed();
        FutureMapGenerator.startPosition += offSet;
        FutureMapGenerator.GenerateDungeon();
        FutureMapGenerator.startPosition -= offSet;
    }
}
