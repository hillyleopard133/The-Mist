using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum WeaponType
{
    Blunt,
    Sharp,
    Magic
}

[CreateAssetMenu(menuName = "Items/Weapon", fileName = "ItemWeapon")]
public class ItemWeapon : ItemEquipment
{
    [Header("Weapon")] 
    public int damage;
    public int critChance;
    public WeaponType weaponType;
}
