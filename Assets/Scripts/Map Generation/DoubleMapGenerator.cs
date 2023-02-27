using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleMapGenerator : AbstractDungeonGenerator
{
    [SerializeField] private Vector2Int offSet = new Vector2Int(0,360);

    [SerializeReference] AbstractDungeonGenerator FutureMapGenerator;
    [SerializeReference] AbstractDungeonGenerator PresentMapGenerator;
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
