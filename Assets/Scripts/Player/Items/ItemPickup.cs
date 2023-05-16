using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] public ItemAbstract item;
    [SerializeField] public ItemData data;

    private void Start()
    {
        item = AssignItem(data.itemEnum);
    }

    private ItemAbstract AssignItem(Items itemEnum)
    {
        switch (itemEnum)
        {
            case Items.FireFang:
                return new FireDamageItem();
            case Items.SleepingMushroom:
                return new HealingOnStandStilItem();
            case Items.SoldierArm:
                return new AttackSpeedIncrease();
            default :
                return new FireDamageItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            AddItem(collision.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }

    public virtual void AddItem(PlayerController player)
    {
        item.OnPickup(player);
        foreach (ItemList i in player.items)
        {
            Debug.Log(i.item.GetType() + " " + item.GetType());
            if (i.item.GetType() == item.GetType())
            {
                i.stacks += 1;
                UIManagerSingleton.instance.DisplayItem(i);
                UIManagerSingleton.instance.UpdateItemSlotsUi(player.items);
                return;
            }
        }
        var itemListItem = new ItemList(item, data, 1);
        player.items.Add(itemListItem);
        UIManagerSingleton.instance.DisplayItem(itemListItem);
        UIManagerSingleton.instance.UpdateItemSlotsUi(player.items);
    }
}
