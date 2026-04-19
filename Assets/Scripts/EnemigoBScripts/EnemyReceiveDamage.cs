using UnityEngine;

public class EnemyReceiveDamage : MonoBehaviour
{
    [SerializeField] private string projectileLayerName = "projectile";

    private int projectileLayer;

    private void Awake()
    {
        projectileLayer = LayerMask.NameToLayer(projectileLayerName);

        if (projectileLayer == -1)
        {
            Debug.LogWarning($"La capa '{projectileLayerName}' no existe en el proyecto.", this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        NotifyProjectileCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        NotifyProjectileCollision(other.gameObject);
    }

    private void NotifyProjectileCollision(GameObject otherObject)
    {
        if (projectileLayer == -1 || otherObject.layer != projectileLayer)
        {
            return;
        }

        Debug.Log($"{name} detecto una colision con un proyectil: {otherObject.name}", this);
    }
}
