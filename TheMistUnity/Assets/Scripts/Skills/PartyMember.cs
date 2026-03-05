using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/PartyMember", fileName = "PartyMember")]
public class PartyMember : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    [TextArea] public string Description;
    [HideInInspector] public bool IsUnlocked;
    public DamageType WeaponDamageType;
    
    public AttackMove[] Attacks;
    [SerializeField, HideInInspector] bool[] AttackIsUnlocked;
    
    [Header("Stats")]
    public int BaseMaxHealth = 100;
    public int BaseDefence = 10;
    public int BaseAttack = 5;
    public int BaseCritChance = 5;
    public int BaseMaxMana = 20;
    
    [HideInInspector] public int SkillPointsHealth;
    [HideInInspector] public int SkillPointsDefence;
    [HideInInspector] public int SkillPointsAttack;
    [HideInInspector] public int SkillPointsCritChance;
    [HideInInspector] public int SkillPointsMana;
    
    [HideInInspector] public int EquipmentHealth;
    [HideInInspector] public int EquipmentDefence;
    [HideInInspector] public int EquipmentAttack;
    [HideInInspector] public int EquipmentCritChance;
    [HideInInspector] public int EquipmentMana;
    
    //Base + Skills
    [HideInInspector] public int CurrentBaseMaxHealth;
    [HideInInspector] public int CurrentBaseDefence;
    [HideInInspector] public int CurrentBaseAttack;
    [HideInInspector] public int CurrentBaseCritChance;
    [HideInInspector] public int CurrentBaseMaxMana;
    
    //Base + Skills + Equipment
    [HideInInspector] public int CurrentMaxHealth;
    [HideInInspector] public int CurrentDefence;
    [HideInInspector] public int CurrentAttack;
    [HideInInspector] public int CurrentCritChance;
    [HideInInspector] public int CurrentMaxMana;

    public void UnlockPartyMember()
    {
        IsUnlocked = true;
    }

    public List<AttackMove> GetUnlockedAttacks()
    {
        List<AttackMove> unlockedAttacks = new List<AttackMove>();
        for (int i = 0; i < Attacks.Length; i++)
        {
            if(AttackIsUnlocked[i]) unlockedAttacks.Add(Attacks[i]);
        }
        return unlockedAttacks;
    }

    public void IncreaseSkillPoints(AttributeType attributeType, int amount)
    {
        switch (attributeType)
        {
            case AttributeType.Health:
                SkillPointsHealth += amount;
                break;

            case AttributeType.Defence:
                SkillPointsDefence += amount;
                break;

            case AttributeType.Attack:
                SkillPointsAttack += amount;
                break;

            case AttributeType.CritChance:
                SkillPointsCritChance += amount;
                break;

            case AttributeType.Mana:
                SkillPointsMana += amount;
                break;
        }
        CalculateBaseStats();
    }

    public void CalculateBaseStats()
    {
        SkillsManager skillsManager = SkillsManager.Instance;
        CurrentBaseMaxHealth = BaseMaxHealth + SkillPointsHealth * skillsManager.healthIncreasePerLevel;
        CurrentBaseDefence = BaseDefence + SkillPointsDefence * skillsManager.defenceIncreasePerLevel;
        CurrentBaseAttack = BaseAttack + SkillPointsAttack * skillsManager.attackIncreasePerLevel;
        CurrentBaseCritChance = BaseCritChance + SkillPointsCritChance * skillsManager.critIncreasePerLevel;
        CurrentBaseMaxMana = BaseMaxMana + SkillPointsMana * skillsManager.manaIncreasePerLevel;
        CalculateFullStats();
    }
    
    private void CalculateFullStats()
    {
        CurrentMaxHealth = CurrentBaseMaxHealth + EquipmentHealth;
        CurrentDefence = CurrentBaseDefence + EquipmentDefence;
        CurrentAttack = CurrentBaseAttack + EquipmentAttack;
        CurrentCritChance = CurrentBaseCritChance + EquipmentCritChance;
        CurrentMaxMana = CurrentBaseMaxMana + EquipmentMana;
    }
    
    public void EquipItem(InventoryItem item)
    {
        switch (item)
        {
            case ItemArmour armour:
                EquipmentHealth = armour.health;
                EquipmentDefence = armour.defence;
                break;
            case ItemWeapon weapon:
                EquipmentAttack = weapon.damage;
                EquipmentCritChance = weapon.critChance;
                break;
            case ItemScroll scroll:
                EquipmentMana = scroll.mana;
                break;
        }
        CalculateFullStats();
    }

    public void UnEquipItem(InventoryItem item)
    {
        switch (item)
        {
            case ItemArmour:
                EquipmentHealth = 0;
                EquipmentDefence = 0;
                break;
            case ItemWeapon:
                EquipmentAttack = 0;
                EquipmentCritChance = 0;
                break;
            case ItemScroll:
                EquipmentMana = 0;
                break;
        }
        CalculateFullStats();
    }

    private void ClearEquipment()
    {
        EquipmentHealth = 0;
        EquipmentDefence = 0;
        EquipmentAttack = 0;
        EquipmentCritChance = 0;
        EquipmentMana = 0;
    }

    private void ClearSkillPoints()
    {
        SkillPointsHealth = 0;
        SkillPointsDefence = 0;
        SkillPointsAttack = 0;
        SkillPointsCritChance = 0;
        SkillPointsMana = 0;
    }

    private void ClearUnlockedAttacks()
    {
        AttackIsUnlocked = new bool[Attacks.Length];
        AttackIsUnlocked[0] = true;
    }

    public void ResetPartyMember()
    {
        ClearEquipment();
        ClearSkillPoints();
        IsUnlocked = false;
        CalculateBaseStats();
        ClearUnlockedAttacks();
    }

    public PartyMemberData GetData()
    {
        PartyMemberData partyMemberData = new PartyMemberData();
        
        partyMemberData.EquipmentHealth = EquipmentHealth;
        partyMemberData.EquipmentDefence = EquipmentDefence;
        partyMemberData.EquipmentAttack = EquipmentAttack;
        partyMemberData.EquipmentCritChance = EquipmentCritChance;
        partyMemberData.EquipmentMana = EquipmentMana;
        
        partyMemberData.SkillPointsHealth = SkillPointsHealth;
        partyMemberData.SkillPointsDefence = SkillPointsDefence;
        partyMemberData.SkillPointsAttack = SkillPointsAttack;
        partyMemberData.SkillPointsCritChance = SkillPointsCritChance;
        partyMemberData.SkillPointsMana = SkillPointsMana;
        
        partyMemberData.IsUnlocked = IsUnlocked;
        partyMemberData.IsAttackMoveUnlocked = AttackIsUnlocked;

        return partyMemberData;
    }
    
    public void SetData(PartyMemberData partyMemberData)
    {
        EquipmentHealth = partyMemberData.EquipmentHealth;
        EquipmentDefence = partyMemberData.EquipmentDefence;
        EquipmentAttack = partyMemberData.EquipmentAttack;
        EquipmentCritChance = partyMemberData.EquipmentCritChance;
        EquipmentMana = partyMemberData.EquipmentMana;

        SkillPointsHealth = partyMemberData.SkillPointsHealth;
        SkillPointsDefence = partyMemberData.SkillPointsDefence;
        SkillPointsAttack = partyMemberData.SkillPointsAttack;
        SkillPointsCritChance = partyMemberData.SkillPointsCritChance;
        SkillPointsMana = partyMemberData.SkillPointsMana;
        
        IsUnlocked = partyMemberData.IsUnlocked;
        AttackIsUnlocked = partyMemberData.IsAttackMoveUnlocked;
    }
}
