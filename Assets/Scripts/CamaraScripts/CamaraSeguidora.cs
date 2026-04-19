using UnityEngine;

public class CamaraSeguidora : MonoBehaviour
{
    [Header("Seguimiento")]
    [SerializeField] private Transform objetivo;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float suavizado = 0.125f;

    [Header("Límites del Mapa")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Start()
    {
        if (objetivo == null)
        {
            GameObject jugador = GameObject.FindWithTag("Player");
            if (jugador != null) objetivo = jugador.transform;
        }
    }

    void LateUpdate()
    {
        if (objetivo != null)
        {
            Vector3 posicionDeseada = objetivo.position + offset;

            // Limitamos la posición para que no se salga de los bordes
            float clampX = Mathf.Clamp(posicionDeseada.x, minX, maxX);
            float clampY = Mathf.Clamp(posicionDeseada.y, minY, maxY);

            Vector3 posicionLimitada = new Vector3(clampX, clampY, posicionDeseada.z);

            // Movimiento suave
            transform.position = Vector3.Lerp(transform.position, posicionLimitada, suavizado);
        }
    }
}