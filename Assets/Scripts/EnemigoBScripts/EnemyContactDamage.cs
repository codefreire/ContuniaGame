using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float damageInterval = 0.3f;

    private HealthComponent playerHealth;
    private float timer;

    void Update()
    {
        if (playerHealth == null) return;

        timer += Time.deltaTime;
        if (timer >= damageInterval)
        {
            timer = 0f;
            playerHealth.RemoveHealth(damageAmount);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<HealthComponent>();
            timer = damageInterval; // daño inmediato al entrar
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = null;
            timer = 0f;
        }
    }
}
