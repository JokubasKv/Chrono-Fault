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
        foreach (ItemList i in player.items)
        {
            Debug.Log(i.item.GetType() + " " + item.GetType());
            if (i.item.GetType() == item.GetType())
            {
                i.stacks += 1;
                UIManagerSingleton.Instance.UpdateItemSlotsUi(player.items);
                return;
            }
        }
        player.items.Add(new ItemList(item, data, 1));
        UIManagerSingleton.Instance.UpdateItemSlotsUi(player.items);
    }
}
