using Unity.VisualScripting;
using UnityEngine;

public class HealthComponent : MonoBehaviour

{
    [SerializeField] public GameObject deathSpawnPrefab;
    [SerializeField] private int maxHealth = 10;
    public int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void AddHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void RemoveHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (currentHealth <= 0)
        {
            if (deathSpawnPrefab != null)
            {
                Instantiate(deathSpawnPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
