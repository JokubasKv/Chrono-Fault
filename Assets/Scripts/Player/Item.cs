using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item 
{
    public abstract string GiveName();
    public virtual void OnUpdate(PlayerController player, int stacks)
    {

    }
    public virtual void OnHit(PlayerController player, Enemy enemy, int stacks)
    {

    }
    public virtual void OnCreate(PlayerController player, Transform target)
    {

    }
}
public class HealingItem : Item
{
    public override string GiveName()
    {
        return "Healing Item";
    }

    public override void OnUpdate(PlayerController player, int stacks)
    {
        player.health.GetHeal( 5 + (2 * stacks));
    }
}

public class FireDamageItem : Item
{
    GameObject effect;
    public override string GiveName()
    {
        return "Fire Damage Item";
    }

    public override void OnHit(PlayerController player, Enemy enemy, int stacks)
    {
        enemy.GetComponent<Health>().GetHit(10 + (10 * stacks), player.gameObject);
    }
    public override void OnCreate(PlayerController player, Transform target)
    {
        if (effect == null) effect = (GameObject)Resources.Load("Particles/FireParticles", typeof(GameObject));
        GameObject fireEffect = GameObject.Instantiate(effect, target);
    }
}