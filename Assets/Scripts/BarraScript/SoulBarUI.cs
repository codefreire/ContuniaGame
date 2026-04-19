using UnityEngine;
using UnityEngine.UI;

public class SoulBarUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image fillImage;

    [Header("Ajustes de Nivel")]
    [SerializeField] private int almasParaNivelMax = 30; // El máximo que definiste

    private void Start()
    {
        // Escuchamos al SoulManager apenas inicie el juego
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.OnSoulsChanged += ActualizarBarra;
            // Actualización inicial
            ActualizarBarra(SoulManager.Instance.currentSouls);
        }
    }

    private void OnDestroy()
    {
        // Importante desuscribirse para evitar errores al cambiar de escena
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.OnSoulsChanged -= ActualizarBarra;
        }
    }

    private void ActualizarBarra(int almasActuales)
    {
        if (fillImage != null)
        {
            // Calculamos el porcentaje basado en 30
            float porcentaje = (float)almasActuales / almasParaNivelMax;
            fillImage.fillAmount = Mathf.Clamp01(porcentaje);
        }
    }
}