using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCone : MonoBehaviour
{
    public float freezeDuration = 3f; 
    public LayerMask enemyLayer;      
    public float lifetime = 1f;

    private void Start()
    {       
        StartCoroutine(DestroyAfterLifetime());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Freeze(freezeDuration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(2 * transform.localScale.x, 2 * transform.localScale.y, 0));
    }


    private IEnumerator DestroyAfterLifetime()
    {
       
        yield return new WaitForSeconds(lifetime);

      
        Destroy(gameObject);
    }
}
