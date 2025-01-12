using System.Collections;
using UnityEngine;

public class WebProjectile : MonoBehaviour
{
    public GameObject webAreaPrefab;
    private LayerMask playerLayer;
    private bool canCollide = false; 
    public float lifetime = 2f;

    private void Start()
    {
       
        StartCoroutine(DestroyAfterLifetime());
    }

    public void Initialize(GameObject webArea, LayerMask playerLayerMask)
    {
        webAreaPrefab = webArea;
        playerLayer = playerLayerMask;
        StartCoroutine(EnableCollisionAfterDelay(0.1f)); 
    }

    private IEnumerator EnableCollisionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canCollide = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canCollide) return; 

        
        Instantiate(webAreaPrefab, transform.position, Quaternion.identity);

       
        if (collision.collider.CompareTag("Player"))
        {
            PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.moveSpeed *= 0.5f; 
            }
        }

        Destroy(gameObject); 
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);

    
        Destroy(gameObject);
    }
}
