using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage = 20;  
    public float explosionRadius = 4f;  
    public float lifetime = 0.5f;  
    void Start()
    {
        
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var enemyCollider in enemiesInRange)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject, lifetime);
    }
}
