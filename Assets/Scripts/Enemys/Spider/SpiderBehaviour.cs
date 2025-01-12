using System.Collections;
using UnityEngine;

public class SpiderBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float jumpForce = 10f;
    public float jumpForwardForce = 5f;

    [Header("Detection Settings")]
    public float detectionRadius = 7f;
    public LayerMask playerLayer;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall and Edge Detection")]
    public float wallCheckDistance = 0.5f; 
    public Transform edgeCheck; 
    public float edgeDetectionRadius = 0.2f;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isPlayerDetected = false;
    private bool isGrounded = false;
    private bool isOnEdge = false;
    private bool isJumping = false;

    public GameObject webProjectilePrefab;
    public GameObject webAreaPrefab;
    public float webShootInterval = 2f;

    private bool isWandering = false;
    private bool isShootingWeb = false;

    private Enemy enemy;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Wander());
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (enemy.IsFrozen()) return;

        DetectPlayer();
        CheckGrounded();
        CheckEdge();

        if (isPlayerDetected)
        {
            if (IsObstacleAhead() && isGrounded && !isJumping)
            {
                Jump();
            }
            else
            {
                ChasePlayer();
            }

            if (!isShootingWeb)
            {
                StartCoroutine(ShootWeb());
            }
        }
        else if (!isWandering)
        {
            StartCoroutine(Wander());
        }
    }

    private void FixedUpdate()
    {
        if (!isJumping && !isPlayerDetected)
        {
            rb.velocity = new Vector2(moveSpeed * (transform.localScale.x > 0 ? 1 : -1), rb.velocity.y);
        
        }
        if (isJumping && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.AddForce(new Vector2(0, jumpForce * 1.5f), ForceMode2D.Impulse);
        }
        if (!isPlayerDetected && !isJumping)
        {
          
            rb.velocity = new Vector2(moveSpeed * (transform.localScale.x > 0 ? 1 : -1), rb.velocity.y);
        }
    }

    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        isPlayerDetected = player != null;
        if (isPlayerDetected)
        {
            playerTransform = player.transform;
        }
        else
        {
            playerTransform = null;
        }
    }

    private void ChasePlayer()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
           
            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void CheckGrounded()
    {
        Vector2 groundCheckPos = new Vector2(groundCheck.position.x, groundCheck.position.y - 0.1f);
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, groundLayer);

        if (isGrounded && isJumping)
        {
            ResetJump(); 
        }
    }

    private bool IsObstacleAhead()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void CheckEdge()
    {
        isOnEdge = !Physics2D.OverlapCircle(edgeCheck.position, edgeDetectionRadius, groundLayer);
    }

    private void Jump()
    {
        if (isJumping) return; 
        isJumping = true;


        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        Invoke(nameof(ResetJump), 0.5f);
    }



    private void ResetJump()
    {
        isJumping = false;
    }

    private IEnumerator Wander()
    {
        isWandering = true;

        while (!isPlayerDetected)
        {
            float direction = Random.Range(-1f, 1f);
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

            yield return new WaitForSeconds(1f); 
        }

        rb.velocity = Vector2.zero;
        isWandering = false;
    }

    private IEnumerator ShootWeb()
    {
        isShootingWeb = true;

        while (isPlayerDetected)
        {
            if (playerTransform != null)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;

                // Instantiate web projectile
                GameObject webProjectile = Instantiate(webProjectilePrefab, transform.position, Quaternion.identity);
                Rigidbody2D webRb = webProjectile.GetComponent<Rigidbody2D>();
                if (webRb != null)
                {
                    webRb.velocity = direction * 25f; 
                }

                webProjectile.GetComponent<WebProjectile>().Initialize(webAreaPrefab, playerLayer);
            }

            yield return new WaitForSeconds(webShootInterval); 
        }

        isShootingWeb = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualize ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Edge check visualization
        if (edgeCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(edgeCheck.position, edgeDetectionRadius);
        }

        // Wall detection visualization
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left) * wallCheckDistance);
    }
}
