using UnityEngine;

public class TeletransportMark : MonoBehaviour
{
    [Header("Recoleccion")]
    [SerializeField] private float collectDistance = 1.5f;
    private SoulManager soulManager;
    private PlayerController playerController;
    [SerializeField]
    public int soulValue = 1;

    private Transform playerTransform;

    private void Awake()
    {
        GameObject soulManagerObj = GameObject.Find("SoulManager");
        if (soulManagerObj != null)
        {
            soulManager = soulManagerObj.GetComponent<SoulManager>();
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<PlayerController>();
        }
    }

    private void OnMouseDown()
    {
        if (playerController == null)
        {
            return;
        }

        playerController.TeleportTo(transform.position);
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= collectDistance)
        {
            soulManager?.AddSouls(soulValue);
            Debug.Log("me cogiste");
            Debug.Log(soulManager == null);
            Destroy(gameObject);
        }
    }
}