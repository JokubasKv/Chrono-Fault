
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int currentHealth, maxHealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    [SerializeField]
    private bool isDead = false;
    public void UpdateBar()
    {
        UIManagerSingleton.Instance.SetHealthBar(currentHealth / (float)maxHealth);
    }

    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue/2;
        maxHealth = healthValue;
        isDead = false;
        UpdateBar();
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
            OnHitWithReference?.Invoke(sender);
        }
        else
        {
            OnDeathWithReference?.Invoke(sender);
            isDead = true;
            UIManagerSingleton.Instance.GameOver();
        }
        UpdateBar(); 
    }
    public void GetHeal(int amount)
    {
        currentHealth += amount;
        if(currentHealth> maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateBar();
    }
}
