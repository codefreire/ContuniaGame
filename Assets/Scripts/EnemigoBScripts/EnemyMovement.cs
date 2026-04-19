using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    [SerializeField] private float poolBallTime = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    [SerializeField]
    public SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isPoolBall = false;
    private float poolBallTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.Play(0, 0, Random.value); // Iniciar animación en un punto aleatorio
        // Buscamos al jugador por su Tag
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    public void EnterPoolBallState(Vector2 velocity)
    {
        isPoolBall = true;
        poolBallTimer = poolBallTime;
        rb.linearVelocity = velocity;
    }

    void Update()
    {
        if (!isPoolBall) return;

        poolBallTimer -= Time.deltaTime;
        if (poolBallTimer <= 0f)
        {
            isPoolBall = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (isPoolBall) return;

        if (player == null || rb == null)
        {
            Debug.Log("Player or Rigidbody2D not found.");
        }
        if (player != null && rb != null)
        {
            // Se mueve constantemente hacia la posición del jugador usando Rigidbody2D
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            Debug.Log(direction);
            spriteRenderer.flipX = direction.x > 0; // Voltear sprite según dirección
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPoolBall) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyMovement otherEnemy = other.GetComponent<EnemyMovement>();
            if (otherEnemy != null)
            {
                otherEnemy.EnterPoolBallState(rb.linearVelocity);
                rb.linearVelocity = Vector2.zero;
                isPoolBall = false;
            }
        }
    }
}