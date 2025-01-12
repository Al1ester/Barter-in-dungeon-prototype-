using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private Dictionary<string, int> materials = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeInventory()
    {
        InventoryManager.Instance.AddMaterial("slime", 20);
        InventoryManager.Instance.AddMaterial("venom", 9);
        InventoryManager.Instance.AddMaterial("metal", 10);
        InventoryManager.Instance.AddMaterial("coal", 3);      
        InventoryManager.Instance.AddMaterial("oil", 0);       
        InventoryManager.Instance.AddMaterial("water", 0);
    }

    public void AddMaterial(string material, int amount)
    {
        if (materials.ContainsKey(material))
        {
            materials[material] += amount;
        }
        else
        {
            materials[material] = amount;
        }
        Debug.Log($"Added {amount} of {material}. Total: {materials[material]}");
    }

    public bool UseMaterial(string material, int amount)
    {
        if (materials.ContainsKey(material) && materials[material] >= amount)
        {
            materials[material] -= amount;
            Debug.Log($"Used {amount} of {material}. Remaining: {materials[material]}");
            return true;
        }
        else
        {
            Debug.LogWarning($"Insufficient {material}. Required: {amount}, Available: {materials.GetValueOrDefault(material, 0)}");
            return false;
        }
    }

    public int GetMaterialCount(string material)
    {
        return materials.GetValueOrDefault(material, 0);
    }

    public List<string> GetLootItems()
    {
        
        return new List<string>(materials.Keys);
    }

    public void UseMaxHealthItem()
    {
       
        if (materials.ContainsKey("maxHealthItem") && materials["maxHealthItem"] > 0)
        {
       
            UseMaterial("maxHealthItem", 1);

      
            PlayerHealth.Instance.AddMaxHealth(1);

            Debug.Log("Max Health increased by 1!");
        }
        else
        {
            Debug.LogWarning("No Max Health item in inventory!");
        }
    }

}
