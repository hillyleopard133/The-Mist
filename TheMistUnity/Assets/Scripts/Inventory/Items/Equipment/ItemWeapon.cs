using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon", fileName = "ItemWeapon")]
public class ItemWeapon : ItemEquipment
{
    [Header("Weapon")] 
    public int damage;
    public int critChance;
}
