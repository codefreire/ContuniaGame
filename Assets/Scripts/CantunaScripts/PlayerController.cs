using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float mouseFollowSpeed = 400f;
    [SerializeField]
    private float arrivalDistance = 0.05f;
    [SerializeField]
    public GameObject brick;
    [SerializeField]
    public GameObject SpawnLeft;
    public GameObject SpawnRight;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D myRigidBody;
    private Camera mainCamera;
    private bool facingLeft = true;
    private bool hasMoveTarget = false;
    private Vector2 targetPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myRigidBody = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        targetPosition = myRigidBody != null ? myRigidBody.position : (Vector2)transform.position;
    }

    void Update()
    {
        if (mainCamera == null || myRigidBody == null)
        {
            return;
        }

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);

        // (Click Izquierdo) Mientras se mantiene, seguir al mouse.
        if (Input.GetMouseButton(0))
        {
            targetPosition = mouseWorld;
            hasMoveTarget = true;
            UpdateFacing(targetPosition - myRigidBody.position);
        }
        else
        {
            hasMoveTarget = false;
        }

        // (Click Derecho) Atacar
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 directionToMouse = (mouseWorld - (Vector2)transform.position).normalized;
            UpdateFacing(directionToMouse);

            GameObject spawnPoint = facingLeft ? SpawnLeft : SpawnRight;
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
        if (myRigidBody == null || !hasMoveTarget)
        {
            return;
        }

        Vector2 currentPosition = myRigidBody.position;
        Vector2 offsetToTarget = targetPosition - currentPosition;

        if (offsetToTarget.sqrMagnitude <= arrivalDistance * arrivalDistance)
        {
            myRigidBody.MovePosition(targetPosition);
            hasMoveTarget = false;
            return;
        }

        UpdateFacing(offsetToTarget);

        myRigidBody.MovePosition(
            Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                mouseFollowSpeed * Time.fixedDeltaTime
            )
        );
    }

    private void UpdateFacing(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) <= Mathf.Epsilon)
        {
            return;
        }

        bool newFacingLeft = direction.x < 0;
        if (newFacingLeft != facingLeft)
        {
            facingLeft = newFacingLeft;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !facingLeft;
            }
        }
    }

    public void TeleportTo(Vector3 destination)
    {
        hasMoveTarget = false;

        Vector3 nextPosition = transform.position;
        nextPosition.x = destination.x;
        nextPosition.y = destination.y;

        if (myRigidBody != null)
        {
            myRigidBody.position = nextPosition;
        }

        transform.position = nextPosition;
        targetPosition = nextPosition;
    }
}