using System;
using UnityEngine.Serialization;

[Serializable]
public class PartyMemberData
{ 
    public int EquipmentHealth;
    public int EquipmentDefence;
    public int EquipmentAttack;
    public int EquipmentCritChance;
    public int EquipmentMana;
    
    public int SkillPointsHealth;
    public int SkillPointsDefence;
    public int SkillPointsAttack;
    public int SkillPointsCritChance;
    public int SkillPointsMana;

    public bool IsUnlocked;

    public bool[] IsAttackMoveUnlocked;
}
