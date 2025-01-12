using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f; 
    public float jumpHoldForce = 2.5f; 
    public float jumpHoldDuration = 0.2f; 
    private float originalMoveSpeed;
    public int slowEffectCounter = 0;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Detection")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isDashing;
    private float dashCooldownTimer;
    private Vector2 dashDirection;

    public Sprite lanternSprite;  
    public Sprite normalSprite;   
    private SpriteRenderer spriteRenderer;

    private bool canDoubleJump = false;
    private bool isJumping;
    private float jumpTimeCounter;

    private int jumpCount = 0;
    public int maxJumps = 2; 

    public bool IsFacingLeft()
    {
        return transform.localScale.x < 0; 
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMoveSpeed = moveSpeed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdatePlayerSprite();
    }
    public void UpdatePlayerSprite()
    {
        
        bool hasLantern = InventoryManager.Instance.GetMaterialCount("lantern") > 0;

        
        if (hasLantern)
        {
            spriteRenderer.sprite = lanternSprite;
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
        }
    }
    private void Update()
    {
     
        UpdateCollisionStates();

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockedBack = false; 
            return; 
        }

        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
            HandleDash();
        }

  
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    public void ApplyKnockback(Vector2 knockbackForce, float duration)
    {
        isKnockedBack = true;
        knockbackTimer = duration;

        
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = dashDirection * dashSpeed;
        }
    }

    private void UpdateCollisionStates()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        
        Vector2 wallCheckDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, wallCheckDirection, wallCheckDistance, groundLayer);

        
        isTouchingWall = wallHit.collider != null && !isGrounded;
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (isTouchingWall && !isGrounded)
        {
           
            moveInput *= 0.2f; 
        }

        Vector2 targetVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;

        
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                
                jumpCount = 1;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (jumpCount < maxJumps && InventoryManager.Instance.GetMaterialCount("rocketboots") > 0)
            {
                
                jumpCount = 2;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            jumpTimeCounter = jumpHoldDuration; 
        }

        if (Input.GetButton("Jump") && jumpCount > 0)
        {
            if (jumpTimeCounter > 0)
            {
               
                rb.velocity = new Vector2(rb.velocity.x, jumpForce + jumpHoldForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {  
                jumpCount = 0;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpCount = 0;

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        if (isGrounded)
        {
            jumpCount = 0;
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing)
        {
            isDashing = true;
            dashCooldownTimer = dashCooldown;

            dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0).normalized;

            if (dashDirection == Vector2.zero)
            {
                dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            }

            Invoke("EndDash", dashDuration);
        }
    }

    private void EndDash()
    {
        isDashing = false;
    }

    public Vector2 GetFacingDirection()
    {
        if (transform.localScale.x > 0) 
            return Vector2.right;
        else
            return Vector2.left; 
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3.right * (transform.localScale.x > 0 ? 1 : -1) * wallCheckDistance));
        }
    }

    public void ApplySlow()
    {
        slowEffectCounter++;
        UpdateSpeed();
    }

    public void RemoveSlow()
    {
        slowEffectCounter = Mathf.Max(0, slowEffectCounter - 1); 
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (slowEffectCounter > 0)
        {
            moveSpeed = originalMoveSpeed * 0.3f; 
        }
        else
        {
            moveSpeed = originalMoveSpeed; 
        }
    }
}
