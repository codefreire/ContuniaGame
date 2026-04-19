using UnityEngine;

public class TeletransportMark : MonoBehaviour
{
    public Transform target; // Arrastra aquí a Cantuña_0

    // El offset mantiene la cámara en Z = -10
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Límites del Mapa (Importante al hacer zoom)")]
    // Al hacer zoom, tendrás que reajustar estos límites para no ver negro.
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    // Usamos LateUpdate para asegurarnos de que el personaje ya se movió
    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculamos la posición deseada (posición del personaje + offset)
        Vector3 desiredPosition = target.position + offset;

        // 2. Limitamos (Clamp) para que no se salga de las calles de Quito
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // 3. Aplicamos la posición INSTANTÁNEAMENTE (Sin suavizado)
        // Mantenemos offset.z (que debe ser -10)
        transform.position = new Vector3(clampedX, clampedY, offset.z);
    }
}