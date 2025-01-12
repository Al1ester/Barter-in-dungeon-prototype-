using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public string requiredMaterial;
    public int materialCost;
    public GameObject spellPrefab;
    public float cooldown;
    public Sprite materialIcon;
}
