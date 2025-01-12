using UnityEngine;

public class TestProjectileSpawner : MonoBehaviour
{
    public GameObject projectilePrefab; 
    public Transform castPoint; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            float[] angles = { 15f, 30f, 45f };
            foreach (float angle in angles)
            {
                GameObject projectile = Instantiate(projectilePrefab, castPoint.position, Quaternion.identity);
                ArcProjectile arcProjectile = projectile.GetComponent<ArcProjectile>();
                if (arcProjectile != null)
                {
                    arcProjectile.Initialize(Vector2.right, angle);
                }
            }
        }
    }
}
