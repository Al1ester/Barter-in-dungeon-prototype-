using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float damage = 10f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy hit and damage applied!");
            }
        }
    }
}
    