using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    public float knockbackForce = 25f; 

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public static event OnHealthChanged HealthChanged;

    public static PlayerHealth Instance { get; private set; }
    private Rigidbody2D rb;

    private SceneManagerController sceneManagerController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }

        currentHealth = maxHealth;
 
        
    }


    private void Start()
    {
        sceneManagerController = FindObjectOfType<SceneManagerController>();
        if (sceneManagerController == null)
        {
            Debug.LogError("SceneManagerController instance not found! Ensure it exists in the MainMenu scene.");
        }

        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>(); 
        UpdateHealthUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            rb.velocity = Vector2.zero; 
            rb.AddForce(new Vector2(-5f, 0f), ForceMode2D.Impulse); 
            Debug.Log("Horizontal force applied!");
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float horizontalForce = -1, float verticalForce = -1)
    {
        if (isInvincible) return;

        float finalHorizontalForce = horizontalForce > 0 ? horizontalForce : knockbackForce;
        float finalVerticalForce = verticalForce > 0 ? verticalForce : knockbackForce;      

        Debug.Log($"TakeDamage called. Direction: {knockbackDirection}, Horizontal Force: {finalHorizontalForce}, Vertical Force: {finalVerticalForce}");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFrames());

       
        Vector2 knockbackForceVector = new Vector2(
            knockbackDirection.x * finalHorizontalForce,
            knockbackDirection.y * finalVerticalForce
        );

        ApplyKnockback(knockbackForceVector);
    }


    public void DetermineKnockbackDirection(Transform enemyTransform)
    {
        Vector2 knockbackDirection;

        if (transform.position.x < enemyTransform.position.x)
        {
            knockbackDirection = new Vector2(-1, 1);
        }
        else
        {
            knockbackDirection = new Vector2(1, 1);
        }

        Debug.Log($"Knockback Direction: {knockbackDirection}");
        TakeDamage(1, knockbackDirection); 
    }

    private void ApplyKnockback(Vector2 forceVector)
    {
        PlayerMovement movementScript = GetComponent<PlayerMovement>();
        if (movementScript != null)
        {
            Debug.Log($"Applying Knockback: {forceVector}");
            movementScript.ApplyKnockback(forceVector, 0.2f); 
        }
        else
        {
            Debug.LogWarning("PlayerMovement script is missing on the player!");
        }
    }




    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void UpdateHealthUI()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; 
        UpdateHealthUI();
    }



    private void Die()
    {
        sceneManagerController.LoadDeath();
    }
}
