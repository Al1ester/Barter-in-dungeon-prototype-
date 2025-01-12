using UnityEngine;

public class Loot : MonoBehaviour
{
    public string lootType; 
    public int lootAmount; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddMaterial(lootType, lootAmount);
            Debug.Log($"Player collected {lootAmount} of {lootType}");
            Destroy(gameObject); 
        }
    }
}
