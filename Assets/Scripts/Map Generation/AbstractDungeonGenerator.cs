using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractMapGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] public Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected bool clearPreviuosVisualization = true;

    public UnityEvent OnFinishedRoomGeneration;

    public void GenerateDungeon()
    {
        if (clearPreviuosVisualization)
            Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

    public abstract HashSet<Vector2Int> GenerateFloor();

    public abstract void Clear();
}
