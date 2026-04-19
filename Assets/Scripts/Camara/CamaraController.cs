using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private Vector3 cameraZOffset = new Vector3(0f, 0f, -10f);

    [Header("Márgenes (0.5 es el centro)")]
    [Range(0f, 1f)][SerializeField] private float margenHorizontal = 0.2f;
    [Range(0f, 1f)][SerializeField] private float margenVertical = 0.2f;

    [Header("Límites")]
    [SerializeField] private BoxCollider2D mapaLimites;

    private Vector3 currentVelocity = Vector3.zero;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null) target = GameObject.FindWithTag("Player")?.transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float altoMedio = cam.orthographicSize;
        float anchoMedio = altoMedio * cam.aspect;

        // Calculamos la posición deseada
        float offsetX = anchoMedio * (1f - (margenHorizontal * 2f));
        float offsetY = altoMedio * (1f - (margenVertical * 2f));
        Vector3 posicionDeseada = target.position + new Vector3(offsetX, offsetY, 0) + cameraZOffset;

        // Solo aplicamos límites si el objeto existe
        if (mapaLimites != null)
        {
            float minX = mapaLimites.bounds.min.x + anchoMedio;
            float maxX = mapaLimites.bounds.max.x - anchoMedio;
            float minY = mapaLimites.bounds.min.y + altoMedio;
            float maxY = mapaLimites.bounds.max.y - altoMedio;

            // ANTIBLOQUEO: Si el mapa es suficiente grande, limitamos. Si no, dejamos mover.
            if (maxX > minX) posicionDeseada.x = Mathf.Clamp(posicionDeseada.x, minX, maxX);
            if (maxY > minY) posicionDeseada.y = Mathf.Clamp(posicionDeseada.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref currentVelocity, smoothTime);
    }
}