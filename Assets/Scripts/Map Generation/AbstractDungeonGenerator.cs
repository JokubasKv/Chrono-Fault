using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMapGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] public Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected bool clearPreviuosVisualization = true;

    public void GenerateDungeon()
    {
        if(clearPreviuosVisualization)
            tilemapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
