using UnityEngine;

public class ArcProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 5;   
    public float poisonDuration = 2f;
    public float poisonTickRate = 0.3f; 
    public int poisonDamage = 2;

    private Vector2 direction;

  
    public void Initialize(Vector2 playerDirection, float angleOffset)
    {
       
        playerDirection = playerDirection.normalized;

        
        bool isFacingLeft = playerDirection.x < 0;

      
        float adjustedAngleOffset = isFacingLeft ? -angleOffset : angleOffset;

        float baseAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        float finalAngle = baseAngle + adjustedAngleOffset;

       
        float radians = finalAngle * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

    
        transform.rotation = Quaternion.Euler(0, 0, finalAngle);
    }


    void Update()
    {

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                enemy.ApplyPoison(poisonDamage, poisonDuration, poisonTickRate);
            }
            Destroy(gameObject); 
        }
        else if (!collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
