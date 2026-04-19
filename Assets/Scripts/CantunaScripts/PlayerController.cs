using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float mouseFollowSpeed = 400f;
    [SerializeField]
    private float arrivalDistance = 0.05f;
    [Header("Movement Collision")]
    [SerializeField] private LayerMask movementObstacleMask = ~0;
    [SerializeField] private float movementContactOffset = 0.02f;
    [SerializeField]
    public GameObject brick;
    [SerializeField]
    public GameObject SpawnLeft;
    public GameObject SpawnRight;
    [Header("Teleport Pool Stick")]
    [SerializeField] private float teleportKnockbackForce = 18f;
    [SerializeField] private float teleportFlashDuration = 0.08f;
    [SerializeField] private float teleportFlashWidth = 0.16f;
    [SerializeField] private int teleportFlashSegments = 8;
    [SerializeField] private float teleportFlashJitter = 0.2f;
    [SerializeField] private float teleportSplitProbeRadius = 1.35f;
    [SerializeField] private float teleportSplitMinForceMultiplier = 0.35f;
    [SerializeField] private float teleportSplitForceExponent = 1.6f;
    [SerializeField] private LayerMask teleportHitMask = ~0;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D myRigidBody;
    private Camera mainCamera;
    private bool facingLeft = true;
    private bool hasMoveTarget = false;
    private bool isMoving = false;
    private Vector2 targetPosition;
    private Animator myAnimator;
    private ContactFilter2D movementContactFilter;
    private readonly RaycastHit2D[] movementHits = new RaycastHit2D[8];

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mainCamera = Camera.main;
        targetPosition = myRigidBody != null ? myRigidBody.position : (Vector2)transform.position;

        movementContactFilter = new ContactFilter2D
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = movementObstacleMask
        };
    }

    void Update()
    {
        // Animar
        myAnimator.SetBool("isMoving", isMoving);

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
            isMoving = true;
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

        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
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

        float maxStepDistance = mouseFollowSpeed * Time.fixedDeltaTime;
        Vector2 desiredNextPosition = Vector2.MoveTowards(currentPosition, targetPosition, maxStepDistance);
        Vector2 desiredDelta = desiredNextPosition - currentPosition;

        if (desiredDelta.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        Vector2 moveDirection = desiredDelta.normalized;
        float moveDistance = desiredDelta.magnitude;

        int hitCount = myRigidBody.Cast(moveDirection, movementContactFilter, movementHits, moveDistance + movementContactOffset);

        if (hitCount > 0)
        {
            float nearestDistance = float.MaxValue;
            bool hasBlockingHit = false;
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = movementHits[i];
                if (hit.collider == null)
                {
                    continue;
                }

                float hitDistance = hit.distance;
                float approachDot = Vector2.Dot(moveDirection, hit.normal);

                // Ignore overlap/skin hits and surfaces we are not moving into to avoid sticky edges.
                if (hitDistance <= 0.0001f || approachDot >= -0.0001f)
                {
                    continue;
                }

                if (hitDistance < nearestDistance)
                {
                    nearestDistance = hitDistance;
                    hasBlockingHit = true;
                }
            }

            if (hasBlockingHit)
            {
                float safeDistance = Mathf.Max(0f, nearestDistance - movementContactOffset);
                myRigidBody.MovePosition(currentPosition + moveDirection * safeDistance);
                return;
            }
        }

        myRigidBody.MovePosition(desiredNextPosition);
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
        Vector2 startPosition = myRigidBody != null ? myRigidBody.position : (Vector2)transform.position;

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

        Vector2 endPosition = targetPosition;
        ApplyTeleportPoolStick(startPosition, endPosition);
        StartCoroutine(ShowTeleportStrikeFlash(startPosition, endPosition));
    }

    private void ApplyTeleportPoolStick(Vector2 startPosition, Vector2 endPosition)
    {
        if (startPosition == endPosition)
        {
            return;
        }

        Vector2 strikeDirection = (endPosition - startPosition).normalized;
        Vector2 strikeNormal = new Vector2(-strikeDirection.y, strikeDirection.x);
        float lineLength = Vector2.Distance(startPosition, endPosition);

        Collider2D[] nearbyEnemies = Physics2D.OverlapCapsuleAll(
            (startPosition + endPosition) * 0.5f,
            new Vector2(lineLength, Mathf.Max(0.05f, teleportSplitProbeRadius * 2f)),
            CapsuleDirection2D.Horizontal,
            Mathf.Atan2(strikeDirection.y, strikeDirection.x) * Mathf.Rad2Deg,
            teleportHitMask
        );

        if (nearbyEnemies == null || nearbyEnemies.Length == 0)
        {
            return;
        }

        HashSet<EnemyMovement> pushedEnemies = new HashSet<EnemyMovement>();

        foreach (Collider2D hit in nearbyEnemies)
        {
            if (hit == null || !hit.CompareTag("Enemy"))
            {
                continue;
            }

            EnemyMovement enemyMovement = hit.GetComponent<EnemyMovement>();
            if (enemyMovement == null || pushedEnemies.Contains(enemyMovement))
            {
                continue;
            }

            Vector2 enemyPos = hit.bounds.center;
            Vector2 closestOnLine = ClosestPointOnSegment(startPosition, endPosition, enemyPos);
            Vector2 fromLine = enemyPos - closestOnLine;

            if (fromLine.sqrMagnitude > teleportSplitProbeRadius * teleportSplitProbeRadius)
            {
                continue;
            }

            float side = Mathf.Sign(Vector2.Dot(fromLine, strikeNormal));
            if (Mathf.Approximately(side, 0f))
            {
                side = Random.value < 0.5f ? -1f : 1f;
            }

            Vector2 splitDirection = strikeNormal * side;

            float normalizedDistance = Mathf.Clamp01(fromLine.magnitude / Mathf.Max(0.001f, teleportSplitProbeRadius));
            float closeness = 1f - normalizedDistance;
            float closenessCurve = Mathf.Pow(closeness, Mathf.Max(0.01f, teleportSplitForceExponent));
            float forceMultiplier = Mathf.Lerp(
                Mathf.Clamp01(teleportSplitMinForceMultiplier),
                1f,
                closenessCurve
            );

            enemyMovement.EnterPoolBallState(splitDirection * (teleportKnockbackForce * forceMultiplier));
            pushedEnemies.Add(enemyMovement);
        }
    }

    private Vector2 ClosestPointOnSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point)
    {
        Vector2 segment = segmentEnd - segmentStart;
        float segmentSqr = segment.sqrMagnitude;
        if (segmentSqr <= Mathf.Epsilon)
        {
            return segmentStart;
        }

        float t = Mathf.Clamp01(Vector2.Dot(point - segmentStart, segment) / segmentSqr);
        return segmentStart + segment * t;
    }

    private IEnumerator ShowTeleportStrikeFlash(Vector2 startPosition, Vector2 endPosition)
    {
        if (teleportFlashDuration <= 0f || startPosition == endPosition)
        {
            yield break;
        }

        GameObject flashObject = new GameObject("TeleportStrikeFlash");
        LineRenderer lineRenderer = flashObject.AddComponent<LineRenderer>();
        int segmentCount = Mathf.Max(2, teleportFlashSegments);
        lineRenderer.positionCount = segmentCount;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = 999;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.numCapVertices = 4;
        lineRenderer.startWidth = teleportFlashWidth;
        lineRenderer.endWidth = teleportFlashWidth * 0.45f;

        Color electricBlue = new Color(0.15f, 0.75f, 1f, 0.95f);
        Color electricBlueTail = new Color(0.45f, 0.95f, 1f, 0.7f);
        lineRenderer.startColor = electricBlue;
        lineRenderer.endColor = electricBlueTail;

        Vector2 mainDirection = (endPosition - startPosition).normalized;
        Vector2 normal = new Vector2(-mainDirection.y, mainDirection.x);

        float elapsed = 0f;
        while (elapsed < teleportFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / teleportFlashDuration);
            float alpha = 1f - t;

            for (int i = 0; i < segmentCount; i++)
            {
                float segmentT = i / (float)(segmentCount - 1);
                Vector2 basePoint = Vector2.Lerp(startPosition, endPosition, segmentT);
                float wobble = Mathf.Sin((elapsed * 60f) + (i * 1.7f)) * teleportFlashJitter;
                Vector2 jitterOffset = (i == 0 || i == segmentCount - 1) ? Vector2.zero : normal * wobble;
                lineRenderer.SetPosition(i, basePoint + jitterOffset);
            }

            lineRenderer.startColor = new Color(electricBlue.r, electricBlue.g, electricBlue.b, electricBlue.a * alpha);
            lineRenderer.endColor = new Color(electricBlueTail.r, electricBlueTail.g, electricBlueTail.b, electricBlueTail.a * alpha);
            yield return null;
        }

        Destroy(flashObject);
    }
}