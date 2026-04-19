using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    private Rigidbody2D rb;
    [SerializeField]
    public SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.Play(0, 0, Random.value); // Iniciar animación en un punto aleatorio
        // Buscamos al jugador por su Tag
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void FixedUpdate()
    {
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
}