using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item_", menuName = "PCG/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item data:")]
    public string itemName;
    public string description;
    public Sprite sprite;
    public Items itemEnum;
}

public enum Items
{
    FireFang,
    SleepingMushroom,
    SoldierArm,
}