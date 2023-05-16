
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int currentHealth, maxHealth;

    [SerializeField]
    private bool isDead = false;

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void GetHit(int amount, GameObject sender)
    {
        if (isDead)
            return;
        if (sender.layer == gameObject.layer)
            return;

        currentHealth -= amount;

        if (currentHealth > 0)
        {
        }
        else
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
    public void GetHeal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
