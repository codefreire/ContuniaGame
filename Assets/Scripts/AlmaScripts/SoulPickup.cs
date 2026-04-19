using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    [SerializeField] private int soulValue = 1;
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (SoulManager.Instance != null)
            {
                SoulManager.Instance.AddSouls(soulValue);
            }

            Destroy(gameObject);
        }
    }
}