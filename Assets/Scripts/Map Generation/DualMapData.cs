using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualMapData : MonoBehaviour
{
    [SerializeField] public MapData PastMapData;
    [SerializeField] public MapData FutureMapData;

    [SerializeField] public Vector2Int offset;
}
