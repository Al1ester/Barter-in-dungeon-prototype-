using UnityEngine;

public class GargoyleBehaviour : MonoBehaviour
{
    public float speed = 2f; 
    public float detectionRadius = 10f; 
    public GameObject rockPrefab; 
    public float throwInterval = 3f; 

    private Transform player;
    private Vector2 targetPosition;
    private float lastThrowTime;
    private Enemy enemy;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastThrowTime = Time.time;
        ChooseNewTargetPosition();
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.IsFrozen()) return;
        if (player != null)
        {
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRadius)
            {
               
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                
                if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
                {
                    ChooseNewTargetPosition();
                }

               
                if (Time.time - lastThrowTime >= throwInterval)
                {
                    ThrowRock();
                    lastThrowTime = Time.time;
                }
            }
        }
    }

    void ChooseNewTargetPosition()
    {
      
        float xOffset = Random.Range(-5f, 5f);
        float yOffset = Random.Range(2f, 5f);
        targetPosition = new Vector2(player.position.x + xOffset, player.position.y + yOffset);
    }

    void ThrowRock()
    {
       
        GameObject rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        float rockSpeed = 10f;
        Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * rockSpeed;
        }
    }

}
