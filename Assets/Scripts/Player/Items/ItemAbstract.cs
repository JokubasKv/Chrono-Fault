using UnityEngine;

[System.Serializable]
public abstract class ItemAbstract
{
    public virtual void OnUpdate(PlayerController player, int stacks)
    {

    }
    public virtual void OnStandStill(PlayerController player, int stacks)
    {

    }
    public virtual void OnHit(PlayerController player, Enemy enemy, int stacks)
    {

    }
    public virtual void OnCreate(PlayerController player, Transform target)
    {

    }
}