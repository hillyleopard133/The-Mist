using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PartyMember", fileName = "PartyMember")]
public class PartyMember : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;
    
    [Header("Stats")]
    public int BaseMaxHealth = 100;
    public int BaseDefence = 10;
    public int BaseAttack = 5;
    public int BaseCritChance = 5;
    public int BaseMaxMana = 20;
    
    [HideInInspector] public int CurrentMaxHealth;
    [HideInInspector] public int CurrentDefence;
    [HideInInspector] public int CurrentAttack;
    [HideInInspector] public int CurrentCritChance;
    [HideInInspector] public int CurrentMaxMana;
    
    [HideInInspector] public int EquipmentHealth;
    [HideInInspector] public int EquipmentDefence;
    [HideInInspector] public int EquipmentAttack;
    [HideInInspector] public int EquipmentCritChance;
    [HideInInspector] public int EquipmentMana;

    private void CalculateStats()
    {
        CurrentMaxHealth = BaseMaxHealth + EquipmentHealth;
        CurrentDefence = BaseDefence + EquipmentDefence;
        CurrentAttack = BaseAttack + EquipmentAttack;
        CurrentCritChance = BaseCritChance + EquipmentCritChance;
        CurrentMaxMana = BaseMaxMana + EquipmentMana;
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
        CalculateStats();
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
        CalculateStats();
    }
}
