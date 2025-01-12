using UnityEngine;

public class SpiritBehaviour : MonoBehaviour
{
    public float detectionRadius = 9f;
    public float moveSpeed = 2f;
    public LayerMask playerLayer;

    private Transform playerTransform;
    private bool playerDetected = false;

    private Rigidbody2D rb;
    private float floatAmplitude = 0.5f; 
    private float floatFrequency = 2f;   
    private float initialY;

    private Enemy enemy;

 
    private InventoryManager inventoryManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; 
        initialY = transform.position.y; 
        enemy = GetComponent<Enemy>();

      
        inventoryManager = InventoryManager.Instance;
    }

    void Update()
    {
        if (enemy.IsFrozen()) return;

        if (!HasLantern())
        {
            DetectPlayer();
        }

        if (playerDetected && playerTransform != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            Float(); 
        }
    }

    private bool HasLantern()
    {

        return inventoryManager.GetMaterialCount("lantern") > 0;
    }

    private void DetectPlayer()
    {
        if (!playerDetected)
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            if (player != null)
            {
                playerTransform = player.transform;
                playerDetected = true;
            }
        }
    }

    private void Float()
    {
        float newY = initialY + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void MoveTowardsPlayer()
    {

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        Vector3 targetPosition = transform.position + (Vector3)(direction * moveSpeed * Time.deltaTime);

        transform.position = targetPosition;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
   
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
