using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(menuName = "ScriptableObjects/Items/Weapon", fileName = "ItemWeapon")]
public class ItemWeapon : ItemEquipment
{
    [Header("Weapon")] 
    public int damage;
    public int critChance;
    public DamageType weaponDamageType;
}
