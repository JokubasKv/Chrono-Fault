using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public Items itemDrop;

    // Start is called before the first frame update
    void Start()
    {
        item = AssignItem(itemDrop);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            AddItem(collision.GetComponent<PlayerController>());
            Destroy(gameObject);
        }

    }

    public void AddItem(PlayerController player)
    {
        foreach(ItemList i in player.items)
        {
            if(i.name == item.GiveName())
            {
                i.stacks += 1;
                return;
            }
        }
        player.items.Add(new ItemList(item, item.GiveName(), 1));
    }
    public Item AssignItem(Items itemToAssign)
    {
        switch (itemToAssign)
        {
            case Items.HealingItem:
                return new HealingItem();
            case Items.FireDamageItem:
                return new FireDamageItem();
            default:
                return new HealingItem();
        }
    }
}
public enum Items
{
    HealingItem,
    FireDamageItem,
}
