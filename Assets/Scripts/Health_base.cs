using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;

    [SerializeField] private float spawnHealth;

    private float currentHealth;


    // Start is called before the first frame update
    void Awake()
    {
        //Initialize current health
        if (spawnHealth > 0) { 
            currentHealth = spawnHealth;
            clampHealth();
        } else
        {
            currentHealth = maxHealth;
        }
    }
    
    public void takeDamage(GameObject instigator, float damage)
    {
        currentHealth -= damage;
        checkDeath();
    }

    private void checkDeath()
    {
        if (currentHealth < 0)
        {
            Destroy(gameObject);
        }
    }

    private void clampHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}
