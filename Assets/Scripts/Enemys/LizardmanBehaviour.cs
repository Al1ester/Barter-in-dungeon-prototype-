using UnityEngine;
using System.Collections;

public class LizardmanBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpForce = 8f;
    public float dashSpeed = 12f;
    public float dashCooldown = 1.5f;
    private float dashCooldownTimer;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public LayerMask playerLayer;
    private Transform playerTransform;
    private bool isAggroed;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Combat")]
    public GameObject swordSlashPrefab;
    public Transform attackPoint;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    public float meleeRadius = 1.5f; 

    [Header("Wall and Jump Detection")]
    public float wallCheckDistance = 0.5f;
    public float jumpForwardForce = 5f;
    private bool isJumping = false;

    private Rigidbody2D rb;
    private Enemy enemy;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; 
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.IsFrozen()) return;
        DetectPlayer();
        UpdateGroundedState();

        if (isAggroed)
        {
            if (IsObstacleAhead() && isGrounded && !isJumping)
            {
                Jump();
            }
            else
            {
                MoveTowardsPlayer();

                if (dashCooldownTimer <= 0)
                    AttemptDash();

                if (Time.time >= lastAttackTime + attackCooldown)
                    AttemptAttack();
            }
        }

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    private void UpdateGroundedState()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && isJumping)
        {
            ResetJump();
        }
    }


    private void DetectPlayer()
    {
        if (!isAggroed)
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            if (player != null)
            {
                playerTransform = player.transform;
                isAggroed = true;
            }
        }
    }

  

    private void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            float direction = playerTransform.position.x - transform.position.x;
            rb.velocity = new Vector2(Mathf.Sign(direction) * moveSpeed, rb.velocity.y);

            
            transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);
        }
    }

    private void AttemptDash()
    {
        if (playerTransform != null)
        {
            dashCooldownTimer = dashCooldown;

            Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(dashDirection.x * dashSpeed, rb.velocity.y);
        }
    }

    private void AttemptAttack()
    {
        if (playerTransform != null)
        {
            
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > meleeRadius) return;

            lastAttackTime = Time.time;

            
            GameObject swordSlash = Instantiate(swordSlashPrefab, attackPoint.position, Quaternion.identity, attackPoint);

           
            SpriteRenderer spriteRenderer = swordSlash.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                
                bool isFacingLeft = transform.localScale.x < 0;

                spriteRenderer.flipX = isFacingLeft;

               
                swordSlash.transform.localScale = new Vector3(
                    0.3f * (isFacingLeft ? -1 : 1), 
                    0.3f,
                    1
                );
            }
            
            Destroy(swordSlash, 0.2f);
        }
    }
  

    private bool IsObstacleAhead()
    {
        
        Vector2 upperRayOrigin = new Vector2(transform.position.x, transform.position.y);
        Vector2 upperDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D upperHit = Physics2D.Raycast(upperRayOrigin, upperDirection, wallCheckDistance, groundLayer);

       
        Debug.DrawLine(upperRayOrigin, upperRayOrigin + upperDirection * wallCheckDistance, Color.yellow);

        
        Vector2 lowerRayOrigin = new Vector2(transform.position.x, transform.position.y - 0.7f); 
        Vector2 lowerDirection = upperDirection;

        RaycastHit2D lowerHit = Physics2D.Raycast(lowerRayOrigin, lowerDirection, wallCheckDistance, groundLayer);

        Debug.DrawLine(lowerRayOrigin, lowerRayOrigin + lowerDirection * wallCheckDistance, Color.green);

        return upperHit.collider != null || lowerHit.collider != null;
    }




    private void Jump()
    {
        if (isJumping) return; 

        isJumping = true;

        rb.velocity = new Vector2(transform.localScale.x * jumpForwardForce, jumpForce);

        Invoke(nameof(ResetJump), 0.5f); 
    }

    private void ResetJump()
    {
        isJumping = false;
    }


    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);


        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left) * wallCheckDistance);
    }

}
