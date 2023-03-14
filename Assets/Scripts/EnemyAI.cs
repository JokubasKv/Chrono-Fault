using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Enemy enemy;

    [SerializeField] private Transform player;

    [SerializeField] private float chaseDistanceTreshold = 3.6f;
    [SerializeField] private float attackDistanceThreshold = 1f;

    [SerializeField] private float attackDelay = 1;
    private float attackTimePassed = 1;

    private void Start()
    {
        if (enemy == null)
            enemy = GetComponent<Enemy>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null)
        {
            enemy.MovementInput = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(player.position, transform.position);

        if(distance < chaseDistanceTreshold)
        {
            //OnPointer
            if(distance <= attackDistanceThreshold)
            {
                //Attack
                enemy.MovementInput = Vector2.zero;
                if (attackTimePassed >= attackDelay)
                {
                    attackTimePassed = 0;
                    enemy.PerformAttack();
                }
            }
            else
            {
                //Chase
                Vector2 direction = player.position - transform.position;
                enemy.MovementInput = direction.normalized;
            }
        }
        else
        {
            enemy.MovementInput = Vector2.zero;
        }
        if(attackTimePassed < attackDelay)
        {
            attackTimePassed += Time.deltaTime;
        }
    }
}
