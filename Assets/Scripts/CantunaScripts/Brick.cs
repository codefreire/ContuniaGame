using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] public int damage = 1;
    [SerializeField] public float speed = 15f;
    [SerializeField] public float maxTravelDistanceSquared = 500f;
    [SerializeField] public float knockbackForce = 15f;
    [SerializeField] public float splashRadius = 2f;

    private Rigidbody2D rb2d;
    private Collider2D projectileCollider;
    private Vector2 moveDirection;
    private Vector2 spawnPosition;
    private bool hasHit;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 direction)
    {
        spawnPosition = rb2d != null ? rb2d.position : (Vector2)transform.position;
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

        Vector2 currentPosition = rb2d != null ? rb2d.position : (Vector2)transform.position;
        if ((currentPosition - spawnPosition).sqrMagnitude >= maxTravelDistanceSquared)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
        {
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            hasHit = true;

            // Disable further trigger events before destruction at end of frame.
            if (projectileCollider != null)
            {
                projectileCollider.enabled = false;
            }

            HealthComponent health = collision.GetComponent<HealthComponent>();
            if (health != null)
            {
                Debug.Log("te pegue" + damage);
                health.RemoveHealth(damage);
            }

            EnemyMovement enemyMovement = collision.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.EnterPoolBallState(moveDirection * knockbackForce);
            }

            // Splash knockback: all other enemies within radius
            Vector2 hitPoint = collision.ClosestPoint(transform.position);
            Collider2D[] nearby = Physics2D.OverlapCircleAll(hitPoint, splashRadius);
            foreach (Collider2D col in nearby)
            {
                if (col == collision) continue;
                if (!col.CompareTag("Enemy")) continue;
                EnemyMovement other = col.GetComponent<EnemyMovement>();
                if (other == null) continue;
                Vector2 dir = ((Vector2)col.transform.position - hitPoint).normalized;
                if (dir == Vector2.zero) dir = Random.insideUnitCircle.normalized;
                other.EnterPoolBallState(dir * knockbackForce);
            }

            if (rb2d != null)
            {
                rb2d.linearVelocity = Vector2.zero;
            }

            Destroy(gameObject);
        }
    }
}