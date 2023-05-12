using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelExitHole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!RewindManager.Instance.IsBeingRewinded)
        {
            Debug.Log(collision.transform.tag);
            if (collision.transform.tag == "Player")
            {
                LevelsManager.Instance.NextLevel();
            }
        }
    }
}
