using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image materialIcon;
    public TextMeshProUGUI materialCountText;

    [Header("References")]
    public PlayerSpellController playerSpellController; 

    void Update()
    {
        UpdateMaterialDisplay();
    }

    private void UpdateMaterialDisplay()
    {
        if (playerSpellController == null || materialIcon == null || materialCountText == null)
        {
            Debug.LogWarning("UI components or PlayerSpellController not assigned!");
            return;
        }

        Spell currentSpell = playerSpellController.spells[playerSpellController.GetCurrentSpellIndex()];

        string requiredMaterial = currentSpell.requiredMaterial;
        int materialCount = InventoryManager.Instance.GetMaterialCount(requiredMaterial);

        
        materialIcon.sprite = currentSpell.materialIcon;
        materialCountText.text = materialCount.ToString(); 
    }
}
