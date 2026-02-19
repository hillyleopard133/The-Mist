using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Armour", fileName = "ItemArmour")]
public class ItemArmour : InventoryItem
{
    public int health;
    public int defence;
    
    public int equipped = -1;
}
