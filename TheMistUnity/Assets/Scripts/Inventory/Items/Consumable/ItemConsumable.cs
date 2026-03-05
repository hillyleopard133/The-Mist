using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items/Consumable", fileName = "ItemConsumable")]
public class ItemConsumable: InventoryItem
{
    public int HealthValue;
    public int ManaValue;
    public bool IsWholeParty;
    public bool IsRevive;

    public int GetHealthValue()
    {
        if (SkillsManager.Instance.GetSkill(SkillTreeSkills.MakeConsumablesMoreEffective).IsUnlocked)
        {
            return Mathf.RoundToInt(HealthValue + HealthValue * 0.3f);
        }
        return HealthValue;
    }

    public int GetManaValue()
    {
        if (SkillsManager.Instance.GetSkill(SkillTreeSkills.MakeConsumablesMoreEffective).IsUnlocked)
        {
            return Mathf.RoundToInt(ManaValue + ManaValue * 0.3f);
        }
        return ManaValue;
    }
}