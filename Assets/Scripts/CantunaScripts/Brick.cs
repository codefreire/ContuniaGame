using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] public int damage = 1;
    [SerializeField] public float speed = 15f;

    private Rigidbody2D rb2d;
    private Vector2 moveDirection;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        if (moveDirection == Vector2.zero)
        {
            moveDirection = Vector2.right;
        }
        // Rotate sprite to face movement direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FixedUpdate()
    {
        rb2d.linearVelocity = moveDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HealthComponent health = collision.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.RemoveHealth(damage);
            }
            Destroy(gameObject);
        }
    }
}