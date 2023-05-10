using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!RewindManager.Instance.IsBeingRewinded)
        {
            if(collision.gameObject.layer == gameObject.layer) 
            {
                return;
            }
            if (collision.transform.tag == "Player")
            {
                PlayerHealth health;
                if (health = collision.gameObject.GetComponent<PlayerHealth>())
                {
                    health.GetHit(5, transform.gameObject);
                }
            }
            Destroy(gameObject);
        }
    }
}
