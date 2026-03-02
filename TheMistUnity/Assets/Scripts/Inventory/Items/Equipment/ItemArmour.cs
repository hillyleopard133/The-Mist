using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items/Armour", fileName = "ItemArmour")]
public class ItemArmour : ItemEquipment
{
    public int health;
    public int defence;
}
