using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items/Scroll", fileName = "ItemScroll")]
public class ItemScroll : ItemEquipment
{
    public int mana;
    public DamageType scrollDamageType;
}
