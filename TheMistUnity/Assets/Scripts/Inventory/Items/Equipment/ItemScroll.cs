using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScrollType
{
    None,
    Fire,
    Ice,
    Wind
}

[CreateAssetMenu(menuName = "Items/Scroll", fileName = "ItemScroll")]
public class ItemScroll : ItemEquipment
{
    public int mana;
    public ScrollType scrollType;
}
