using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item 
{
    public abstract string GiveName();
    public virtual void Update(PlayerController player, int stacks)
    {

    }
    public virtual void OnHit(PlayerController player, Enemy enemy, int stacks)
    {

    }
}
public class HealingItem : Item
{
    public override string GiveName()
    {
        return "Healing Item";
    }

    public override void Update(PlayerController player, int stacks)
    {
        player.currentHealth += 5;
    }
}

public class FireDamageItem : Item
{
    public override string GiveName()
    {
        return "Fire Damage Item";
    }

    public override void OnHit(PlayerController player, Enemy enemy, int stacks)
    {
    }
}