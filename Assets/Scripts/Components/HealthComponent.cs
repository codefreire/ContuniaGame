using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            if (gameObject.CompareTag("Player"))
            {               // Aquí puedes agregar lógica adicional para el jugador, como mostrar una pantalla de Game Over
                SceneManager.LoadScene(0);
            }
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
