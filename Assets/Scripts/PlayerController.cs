using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float mouseFollowSpeed = 400f;
    [SerializeField]
    public GameObject brick;
    [SerializeField]
    public GameObject SpawnLeft;
    public GameObject SpawnRight;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    private Rigidbody2D myRigidBody;
    private bool facingLeft = true;
    private bool isDragging = false;
    private Vector2 targetPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector2 directionToMouse = (mouseWorld - (Vector2)transform.position).normalized;

        if (spriteRenderer != null && isDragging == false)
        {
            spriteRenderer.flipX = directionToMouse.x > 0;
            
        }
        facingLeft = directionToMouse.x < 0;

        // (Click Izquierdo) Arrastrar
        if (Input.GetMouseButtonDown(0))
        {
            if (myCollider != null && myCollider.OverlapPoint(mouseWorld))
            {
                isDragging = true;
            }
        }

        // Arrastrarse
        if (isDragging)
        {
            targetPosition = mouseWorld;
        }

        // (Soltar Click Izquierdo) Dejar de arrastrarse
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // (Click Derecho) Atacar
        if (Input.GetMouseButtonDown(1))
        {
            GameObject spawnPoint = facingLeft? SpawnLeft: SpawnRight;
             Vector2 spawnPosition = spawnPoint.transform.position;

            GameObject attackObj = Instantiate(
                brick,
                spawnPosition,
                Quaternion.identity
            );

            Brick brickScript = attackObj.GetComponent<Brick>();
            if (brickScript != null)
            {
                brickScript.Initialize(directionToMouse);
            }
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            myRigidBody.MovePosition(
                Vector2.MoveTowards(
                    myRigidBody.position,
                    targetPosition,
                    mouseFollowSpeed * Time.fixedDeltaTime
                )
            );
        }
    }
}