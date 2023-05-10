using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamageItem : ItemAbstract
{
    GameObject effect;
    public override void OnHit(PlayerController player, Enemy enemy, int stacks)
    {
        enemy.GetComponent<EnemyHealth>().GetHit(5 + (5 * stacks), player.gameObject);
    }
    public override void OnCreate(PlayerController player, Transform target)
    {
        if (effect == null) effect = (GameObject)Resources.Load("Particles/FireParticles", typeof(GameObject));
        GameObject fireEffect = GameObject.Instantiate(effect, target);
    }
}