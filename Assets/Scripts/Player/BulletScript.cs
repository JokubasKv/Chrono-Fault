using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!RewindManager.instance.IsBeingRewinded)
        {
            if (collision.transform.tag == "Enemy")
            {
                EnemyHealth health;
                if (health = collision.gameObject.GetComponent<EnemyHealth>())
                {
                    health.GetHit(1, transform.gameObject);
                    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().CallItemOnHit(enemy);
                }
            }
            Destroy(gameObject);
        }
    }
}
