using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RectangularRoomParameters_", menuName = "PCG/RectangularRoomData")]
public class RectangularRoomData : ScriptableObject
{
    public int minX = 5, maxX = 10;
    public int minY = 5, maxY = 10;
}
