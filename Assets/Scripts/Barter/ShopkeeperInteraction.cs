using UnityEngine;

public class ShopkeeperInteraction : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private Shopkeeper shopkeeper;

    private bool isPlayerNearby = false;

    private void Awake()
    {
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>();

            if (shopManager == null)
            {
                Debug.LogError("ShopManager not found in the scene. Please add it.");
            }
        }

        shopkeeper.LoadRelationshipStatus(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log($"Press 'E' to open the shop. Relationship status: {shopkeeper.relationshipStatus}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            shopManager.CloseShopMenu();
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            shopManager.currentShopkeeper = shopkeeper;
            shopManager.OpenShopMenu();
        }
    }

    private void OnDestroy()
    {
        shopkeeper.SaveRelationshipStatus(); 
    }
}
