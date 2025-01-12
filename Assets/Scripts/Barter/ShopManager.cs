using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown playerMaterialDropdown;
    [SerializeField] private TMP_Dropdown shopMaterialDropdown;
    [SerializeField] private TMP_InputField playerMaterialAmountInput;
    [SerializeField] private TMP_Text shopMaterialAmountText;
    [SerializeField] private UnityEngine.UI.Button exchangeButton;
    [SerializeField] private UnityEngine.UI.Button tryDiscountButton;
    [SerializeField] private UnityEngine.UI.Button giveLootButton;
    [SerializeField] private TMP_Text relationshipStatusText;
    [SerializeField] private TMP_Text discountChanceText;
    [SerializeField] private TMP_Dropdown lootDropdown;
    [SerializeField] private TMP_InputField lootAmountInput;
    [SerializeField] private GameObject shopMenu;

    public Shopkeeper currentShopkeeper; 
    private bool isShopOpen = false;

    private void Start()
    {                  
        exchangeButton.onClick.AddListener(HandleExchange);
        tryDiscountButton.onClick.AddListener(HandleTryDiscount);
        giveLootButton.onClick.AddListener(HandleGiveLoot);
        playerMaterialAmountInput.onValueChanged.AddListener(UpdateShopMaterialAmount);
        CloseShopMenu();
    }

    private void PopulateDropdowns()
    {

        playerMaterialDropdown.ClearOptions();
        List<string> playerMaterials = new List<string> { "slime", "venom", "metal" };
        playerMaterialDropdown.AddOptions(playerMaterials);

        shopMaterialDropdown.ClearOptions();

       
        Debug.Log("Exchange Rates: " + string.Join(", ", currentShopkeeper.exchangeRates.Select(r => r.shopMaterial).ToArray()));

        List<string> shopMaterials = currentShopkeeper.exchangeRates
            .Select(rate => rate.shopMaterial)
            .Distinct()
            .ToList();

        Debug.Log("Shopkeeper's shop materials: " + string.Join(", ", shopMaterials));

        shopMaterialDropdown.AddOptions(shopMaterials);

        lootDropdown.ClearOptions();
        List<string> lootItems = InventoryManager.Instance.GetLootItems();  
        lootDropdown.AddOptions(lootItems);
    }





    private void UpdateShopMaterialAmount(string input)
    {
        if (int.TryParse(input, out int playerAmount))
        {
           
            if (playerMaterialDropdown.options.Count > 0 && shopMaterialDropdown.options.Count > 0)
            {            
                string playerMaterial = playerMaterialDropdown.options[playerMaterialDropdown.value].text;
                string shopMaterial = shopMaterialDropdown.options[shopMaterialDropdown.value].text;

                
                int exchangeRate = currentShopkeeper.GetExchangeRate(playerMaterial, shopMaterial);

                if (exchangeRate > 0)
                {
                    
                    int shopAmount = CalculatePrice(playerAmount, playerMaterial, shopMaterial, exchangeRate);
                    shopMaterialAmountText.text = shopAmount.ToString();
                }
                else
                {
                    shopMaterialAmountText.text = "No exchange rate available.";
                }
            }
            else
            {
                shopMaterialAmountText.text = "No available materials in the dropdown.";
            }
        }
        else
        {
            shopMaterialAmountText.text = "0";
        }

        UpdateTryDiscountButton();
        UpdateDiscountChanceUI();
    }




    private int CalculatePrice(int playerAmount, string playerMaterial, string shopMaterial, int exchangeRate)
    {
        int priceModifier = currentShopkeeper.relationshipStatus < 30 ? Mathf.Abs(currentShopkeeper.relationshipStatus - 30) / 10 : 0;
        return playerAmount / Mathf.Max(1, exchangeRate + priceModifier);
    }


    private void HandleExchange()
    {
        string playerMaterial = playerMaterialDropdown.options[playerMaterialDropdown.value].text;
        string shopMaterial = shopMaterialDropdown.options[shopMaterialDropdown.value].text;

        if (int.TryParse(playerMaterialAmountInput.text, out int playerAmount))
        {
            if (shopMaterial == "lantern") 
            {
                bool success = InventoryManager.Instance.UseMaterial(playerMaterial, playerAmount);
                if (success)
                {
                    InventoryManager.Instance.AddMaterial("lantern", 1);

                    
                    PlayerMovement player = FindObjectOfType<PlayerMovement>(); 
                    if (player != null)
                    {
                        player.UpdatePlayerSprite(); 
                    }

                    Debug.Log("Lantern purchased!");
                }
            }

            else if (shopMaterial == "cheese")
            {
                int healthUpgradeAmount = 1;
                bool success = InventoryManager.Instance.UseMaterial(playerMaterial, playerAmount);
                if (success)
                {
                   
                    PlayerHealth player = FindObjectOfType<PlayerHealth>(); 
                    if (player != null)
                    {
                        player.AddMaxHealth(healthUpgradeAmount);
                        Debug.Log($"Bought {playerAmount} cheese. Health upgraded by {healthUpgradeAmount}.");
                    }
                    else
                    {
                        Debug.LogError("PlayerHealth component not found.");
                    }
                }
            }

            
            else
            {
                
                int exchangeRate = currentShopkeeper.GetExchangeRate(playerMaterial, shopMaterial);

                if (exchangeRate > 0)
                {
                    int shopAmount = CalculatePrice(playerAmount, playerMaterial, shopMaterial, exchangeRate);

                    if (shopAmount > 0)
                    {
                        bool success = InventoryManager.Instance.UseMaterial(playerMaterial, playerAmount);
                        if (success)
                        {
                            InventoryManager.Instance.AddMaterial(shopMaterial, shopAmount);
                            Debug.Log($"Exchanged {playerAmount} {playerMaterial} for {shopAmount} {shopMaterial}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Invalid exchange amount.");
                    }
                }
            }

            UpdateRelationshipUI();
        }
    }


    private void HandleTryDiscount()
    {
        if (currentShopkeeper == null)
        {
            Debug.LogWarning("No shopkeeper assigned!");
            return;
        }

        string playerMaterial = playerMaterialDropdown.options.Count > 0 ? playerMaterialDropdown.options[playerMaterialDropdown.value].text : null;
        string shopMaterial = shopMaterialDropdown.options.Count > 0 ? shopMaterialDropdown.options[shopMaterialDropdown.value].text : null;

        if (playerMaterial == null || shopMaterial == null)
        {
            Debug.LogWarning("Invalid dropdown selection!");
            return;
        }

        if (int.TryParse(playerMaterialAmountInput.text, out int playerAmount) && playerAmount > 0)
        {
            int exchangeRate = currentShopkeeper.GetExchangeRate(playerMaterial, shopMaterial);

            if (exchangeRate > 0)
            {
                int shopAmount = CalculatePrice(playerAmount, playerMaterial, shopMaterial, exchangeRate);

                if (shopAmount > 0)
                {
                    bool success = InventoryManager.Instance.UseMaterial(playerMaterial, playerAmount);
                    if (success)
                    {
                        bool discountApplied = TryForDiscount();
                        if (discountApplied)
                        {
                            int discountAmount = Mathf.Clamp(Random.Range(2, 5), 0, playerAmount);
                            playerAmount -= discountAmount;

                            InventoryManager.Instance.AddMaterial(shopMaterial, shopAmount);
                            Debug.Log($"Discount applied! Saved {discountAmount} {playerMaterial}.");
                        }
                        else
                        {
                            int penalty = currentShopkeeper.relationshipStatus > 50 ? 3 : 5;
                            currentShopkeeper.relationshipStatus = Mathf.Max(0, currentShopkeeper.relationshipStatus - penalty);

                            InventoryManager.Instance.AddMaterial(shopMaterial, shopAmount);
                            Debug.Log("Discount failed. Relationship decreased.");
                        }
                    }
                }
            }
        }

     
        UpdateRelationshipUI();
    }




    private void HandleGiveLoot()
    {
        string lootItem = lootDropdown.options[lootDropdown.value].text;

        if (lootItem != "slime" && lootItem != "venom" && lootItem != "metal")
        {
            Debug.LogWarning("Invalid loot item selected! Only slime, venom, or metal can be given.");
            return;
        }

        int lootAmount = int.Parse(lootAmountInput.text); 
        bool success = InventoryManager.Instance.UseMaterial(lootItem, lootAmount); 

        if (success)
        {
            int relationshipIncrease = 0;

            
            if (lootItem == "slime" && lootAmount >= 3)
            {
                relationshipIncrease = lootAmount / 3; 
            }
            else if (lootItem == "metal" && lootAmount >= 2)
            {
                relationshipIncrease = lootAmount / 2; 
            }
            else if (lootItem == "venom" && lootAmount >= 2)
            {
                relationshipIncrease = lootAmount / 2; 
            }

            currentShopkeeper.relationshipStatus += relationshipIncrease;

            currentShopkeeper.relationshipStatus = Mathf.Clamp(currentShopkeeper.relationshipStatus, 0, 100);

            Debug.Log($"Gave {lootAmount} {lootItem} to shopkeeper. Relationship increased by {relationshipIncrease}.");
            UpdateRelationshipUI(); 
        }
        else
        {
            Debug.LogWarning("You don't have enough of that loot to give.");
        }
    }


    private bool TryForDiscount()
    {
        int discountChance = Mathf.Clamp(currentShopkeeper.relationshipStatus, 0, 100);
        return Random.Range(0, 100) < discountChance;
    }

    private void UpdateRelationshipUI()
    {
        relationshipStatusText.text = $"Relationship: {currentShopkeeper.relationshipStatus}";
        UpdateTryDiscountButton();
        UpdateDiscountChanceUI();
    }

    private void UpdateTryDiscountButton()
    {
        tryDiscountButton.gameObject.SetActive(currentShopkeeper.relationshipStatus >= 0);
    }

    private void UpdateDiscountChanceUI()
    {
        int discountChance = Mathf.Clamp(currentShopkeeper.relationshipStatus, 0, 100);
        discountChanceText.text = $"Discount Chance: {discountChance}%";
    }



    public void OpenShopMenu()
    {
        PopulateDropdowns();
        shopMenu.SetActive(true);
        Time.timeScale = 0;
        isShopOpen = true;
        UpdateRelationshipUI();
    }

    public void CloseShopMenu()
    {
        shopMenu.SetActive(false);
        Time.timeScale = 1;
        isShopOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShopOpen)
            {
                CloseShopMenu();
            }         
        }
    }


}
