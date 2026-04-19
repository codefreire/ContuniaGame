using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private Image fillImage;
    [SerializeField] private int maxVidaManual = 10; // Debe ser igual al del HealthComponent

    void Update()
    {
        if (healthComponent != null && fillImage != null)
        {
            // El (float) es obligatorio para que la división no dé siempre 0 o 1
            float porcentaje = (float)healthComponent.currentHealth / maxVidaManual;
            fillImage.fillAmount = porcentaje;
        }
        else if (healthComponent == null)
        {
            // Si Cantuña muere y desaparece, la barra se vacía
            fillImage.fillAmount = 0;
        }
    }
}