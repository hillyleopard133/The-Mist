using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Treasure,
    Resource,
    Consumable,
    Equipment,
    Quest
}

public class InventoryItem : ScriptableObject
{
    [Header("Config")]
    public string ID;
    public string Name;
    public Sprite Icon;
    [TextArea] public string Description;
    public int SellValue;
    public int BuyValue;

    [Header("Info")]
    public ItemType ItemType;
    public bool IsConsumable;
    public bool IsStackable;
    public int MaxStack;

    [HideInInspector] public int Quantity;

    public InventoryItem CopyItem()
    {
        InventoryItem instance = Instantiate(this);
        return instance;
    }

    public virtual bool UseItem()
    {
        if (IsConsumable)
        {
            return true;
        }
        return false;
    }

    public virtual void EquipItem()
    {
        
    }

    public virtual void RemoveItem()
    {
        
    }

}
