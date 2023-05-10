using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{ 
    public GameObject imageGameobject;
    public Image image;
    public TextMeshProUGUI text;
    internal void SetUILayout(ItemList itemList)
    {
        imageGameobject.SetActive(true);
        image.sprite = itemList.data.sprite;
        text.text = "x" + itemList.stacks.ToString();
    }
}
