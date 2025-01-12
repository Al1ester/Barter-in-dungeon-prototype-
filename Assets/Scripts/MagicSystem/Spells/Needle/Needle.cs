using System.Collections;
using UnityEngine;

public class Needle : MonoBehaviour
{
    public float speed = 5f; 
    public int damage = 5;   
    public float lifetime = 3f; 
    public float detectionRadius = 22f; 

    private Transform target;
    private float delay;
    private bool hasLaunched = false;

    public void InitializeWithDelay(float startDelay)
    {
        delay = startDelay;
        StartCoroutine(DelayedLaunch());
    }

    private IEnumerator DelayedLaunch()
    {
        yield return new WaitForSeconds(delay);

        target = FindNearestEnemyWithinRadius();
        if (target != null)
        {
            RotateTowardsTarget();
            hasLaunched = true;
        }

        
        StartCoroutine(DestroyAfterLifetime());
    }

    void Update()
    {
        if (!hasLaunched) return;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
           
            target = FindNearestEnemyWithinRadius();
            if (target != null)
            {
                RotateTowardsTarget();
            }
            else
            {
               
                Destroy(gameObject);
                return;
            }
        }

        
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void RotateTowardsTarget()
    {
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Transform FindNearestEnemyWithinRadius()
    {
        float closestDistance = detectionRadius; 
        Transform closestEnemy = null;

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= detectionRadius && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
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
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
