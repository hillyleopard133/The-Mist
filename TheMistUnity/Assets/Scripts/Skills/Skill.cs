using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum SkillTreeSkills
{
    BonusCombatGold,
    BonusCombatExp,
    IncreaseSprintSpeed,
    ShopDiscount,
    ExtraUltimateCharge1,
    ExtraUltimateCharge2,
    ExtraUltimateCharge3,
    IncreaseUltimateChargeSpeed,
    IncreaseBlockTimingWindow,
    IncreaseAttackTimingWindow,
    UnlockFastTravel,
    MakeEnemyHealthVisible,
    MakeEnemyWeaknessesVisible,
    MakeEnemyResistancesVisible,
    MakeConsumablesMoreEffective
}


[CreateAssetMenu(menuName = "ScriptableObjects/Skill", fileName = "Skill")]
public class Skill : ScriptableObject
{
    public string SkillName;
    [TextArea] public string SkillDescription;
    public Sprite SkillIcon;
    public SkillTreeSkills SkillTreeSkill;
    public Skill RequiredSkill;
    public int LevelRequired;
    public int OrbCost;
    public bool IsUnlocked;

    public bool IsAvailable()
    {
        if(SkillsManager.Instance.Level < LevelRequired) return false;
        
        return RequiredSkill.IsUnlocked;
    }

    public bool CanAffordSkill()
    {
        if(OrbCost > SkillsManager.Instance.skillOrbs) return false;
        
        return true;
    }

    public bool HasRequiredSkill()
    {
        return RequiredSkill != null;
    }
}
