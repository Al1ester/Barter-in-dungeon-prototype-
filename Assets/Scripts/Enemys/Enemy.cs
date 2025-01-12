using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 50;
    public float currentHealth;
    private bool isFrozen = false;

    private float freezeTimer = 0f;
    private Coroutine poisonCoroutine;

    public int damage = 1;
    public float knockbackForce = 5f;

   
    public GameObject lootPrefab; 
    public int lootAmount = 1;    

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0)
            {
                Unfreeze();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyPoison(int poisonDamage, float duration, float tickRate)
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }
        poisonCoroutine = StartCoroutine(HandlePoison(poisonDamage, duration, tickRate));
    }

    private IEnumerator HandlePoison(int poisonDamage, float duration, float tickRate)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            TakeDamage(poisonDamage);
            yield return new WaitForSeconds(tickRate);
            elapsed += tickRate;
        }

        poisonCoroutine = null;
    }

    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            isFrozen = true;
            freezeTimer = duration;
        }
    }

    private void Unfreeze()
    {
        isFrozen = false;
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }

    private void Die()
    {
     
        DropLoot();

        Destroy(gameObject);
    }

    private void DropLoot()
    {
        if (lootPrefab != null)
        {
            for (int i = 0; i < lootAmount; i++)
            {
                Vector3 spawnPosition = transform.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f),
                    0);

                GameObject loot = Instantiate(lootPrefab, spawnPosition, Quaternion.identity);
                Loot lootScript = loot.GetComponent<Loot>();

                
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            Debug.Log("Player hit by enemy!");

         
            player.DetermineKnockbackDirection(transform);
        }
    }
}
