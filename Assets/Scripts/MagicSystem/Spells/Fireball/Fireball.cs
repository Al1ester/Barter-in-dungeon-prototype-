using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;  
    public int damage = 20;    
    public float explosionRadius = 4f;  
    public float explosionDelay = 0.2f; 
    public GameObject explosionPrefab; 

    private Vector2 direction;

    public void Initialize(Vector2 playerDirection)
    {
        direction = playerDirection.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {      
        if (collision.CompareTag("Enemy") || (!collision.CompareTag("Player")) )
        {
            TriggerExplosion();
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private void TriggerExplosion()
    {
       
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
      
        Destroy(gameObject);
    }
}
