using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public Vector2 spawnAreaSize = new Vector2(10, 10);

    [Header("Seguridad (Anti-Camp)")]
    public float minSpawnDistance = 4f; // Los enemigos no saldrán a menos de esta distancia
    private Transform playerTransform;

    private float timer;

    void Start()
    {
        // Buscamos al protagonista. ¡RECUERDA ponerle el Tag "Player" en el Inspector!
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("¡Ojo! No encontré al protagonista. Revisa si tiene el Tag 'Player'.");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0;
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = Vector3.zero;
        bool positionValid = false;
        int attempts = 0;
        int maxAttempts = 60; // Límite de intentos para no congelar el juego

        // Si no hay jugador en escena, spawnea en cualquier lado
        if (playerTransform == null)
        {
            spawnPos = GetRandomPosition();
        }
        else
        {
            // Intentamos buscar una posición que esté lejos del jugador
            while (!positionValid && attempts < maxAttempts)
            {
                spawnPos = GetRandomPosition();

                if (Vector2.Distance(spawnPos, playerTransform.position) >= minSpawnDistance)
                {
                    positionValid = true;
                }
                attempts++;
            }
        }

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    // Función auxiliar para calcular la posición aleatoria
    Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return transform.position + new Vector3(randomX, randomY, 0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0));

        // Dibujamos un círculo rojo alrededor del jugador para ver la zona segura en el Editor
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minSpawnDistance);
        }
    }
}