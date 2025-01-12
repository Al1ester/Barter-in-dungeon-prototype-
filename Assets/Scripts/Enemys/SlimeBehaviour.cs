using System.Collections;
using UnityEngine;

public class SlimeBehaviour : MonoBehaviour
{
    public float detectionRadius = 9f; 
    public float hopForce = 7f;       
    public float hopCooldown = 2.5f;  
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isHopping;

    private Enemy enemy;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (enemy.IsFrozen()) return;
        DetectPlayer();
    }

    private void DetectPlayer()
    {

        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (player != null && !isHopping)
        {
            playerTransform = player.transform;
            StartCoroutine(HopRoutine());
        }
    }

    private IEnumerator HopRoutine()
    {
        isHopping = true; 
        HopTowardsPlayer();

        yield return new WaitForSeconds(hopCooldown); 
        isHopping = false;
    }

    private void HopTowardsPlayer()
    {
        if (playerTransform != null)
        {
         
            Vector2 direction = (playerTransform.position - transform.position).normalized;

         
            rb.velocity = Vector2.zero;

            Vector2 initialHopForce = new Vector2(direction.x * hopForce * 0.3f, hopForce);
            rb.AddForce(initialHopForce, ForceMode2D.Impulse);

            StartCoroutine(ApplyHorizontalForce(direction));
        }
    }

    private IEnumerator ApplyHorizontalForce(Vector2 direction)
    {
        yield return new WaitForSeconds(0.2f);

        rb.AddForce(new Vector2(direction.x * hopForce * 0.7f, 0f), ForceMode2D.Impulse);
    }




    private void OnDrawGizmosSelected()
    {
     
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
