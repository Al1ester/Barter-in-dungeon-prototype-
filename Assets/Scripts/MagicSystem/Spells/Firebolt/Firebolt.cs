using UnityEngine;

public class Firebolt : MonoBehaviour
{
    public float speed = 10f;  
    public int damage = 10;    

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
        if (collision.CompareTag("Enemy"))  
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);  
        }
        else if (!collision.CompareTag("Player")) 
        {
            Destroy(gameObject); 
        }
    }
}
