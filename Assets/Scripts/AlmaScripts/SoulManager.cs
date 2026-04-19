using System;
using UnityEngine;

public class SoulManager : MonoBehaviour
{
    public static SoulManager Instance { get; private set; }

    [Header("Configuración inicial")]
    [SerializeField] public int currentSouls = 0;
    [SerializeField] public int totalSoulsCollected = 0;

    // Evento para actualizar UI u otros sistemas
    public event Action<int> OnSoulsChanged;

    private void Awake()
    {
        // Patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NotifyChange();
    }

    // SUMAR almas
    public void AddSouls(int amount)
    {
        if (amount <= 0) return;

        currentSouls += amount;
        totalSoulsCollected += amount;
        NotifyChange();
    }

    // RESTAR almas (para compras, habilidades, etc.)
    public bool SpendSouls(int amount)
    {
        if (amount <= 0) return false;

        if (currentSouls < amount)
        {
            return false; // No alcanza
        }

        currentSouls -= amount;
        NotifyChange();
        return true;
    }

    // VALIDAR si alcanza
    public bool HasEnoughSouls(int amount)
    {
        return currentSouls >= amount;
    }

    // RESET (útil para reiniciar partida)
    public void ResetSouls()
    {
        currentSouls = 0;
        NotifyChange();
    }

    // Método interno para notificar cambios
    private void NotifyChange()
    {
        OnSoulsChanged?.Invoke(currentSouls);
    }
}