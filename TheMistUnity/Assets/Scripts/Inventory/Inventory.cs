using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Inventory : Singleton<Inventory>
{
    [Header("Config")] 
    [SerializeField] public GameContent gameContent;
    [SerializeField] private int inventorySize;
    
    private InventoryItem[] inventoryItemsTreasure;
    private InventoryItem[] inventoryItemsResources;
    private ItemConsumable[] inventoryItemsConsumables;
    private ItemEquipment[] inventoryItemsEquipment;
    private InventoryItem[] inventoryItemsQuests;

    public int InventorySize => inventorySize;
    public InventoryItem[] InventoryItemsTreasure => inventoryItemsTreasure;
    public InventoryItem[] InventoryItemsResources => inventoryItemsResources;
    public ItemConsumable[] InventoryItemsConsumables => inventoryItemsConsumables;
    public ItemEquipment[] InventoryItemsEquipment => inventoryItemsEquipment;
    public InventoryItem[] InventoryItemsQuests => inventoryItemsQuests;
    
    private const string INVENTORY_TREASURE = "Inventory_Treasure";
    private const string INVENTORY_RESOURCES = "Inventory_Resources";
    private const string INVENTORY_CONSUMABLES = "Inventory_Consumables";
    private const string INVENTORY_EQUIPMENT = "Inventory_Equipment";
    private const string INVENTORY_QUESTS = "Inventory_Quests";
    
    private AudioManager audioManager;

    private int currentInventory;
    
    private void Start()
    {
        inventoryItemsTreasure = new InventoryItem[inventorySize];
        inventoryItemsResources = new InventoryItem[inventorySize];
        inventoryItemsConsumables = new ItemConsumable[inventorySize];
        inventoryItemsEquipment = new ItemEquipment[inventorySize];
        inventoryItemsQuests = new InventoryItem[inventorySize];
        
        audioManager = AudioManager.Instance;
    }

    public void SelectInventory(int index)
    {
        currentInventory = index;
    }

    public InventoryItem[] GetCurrentInventory()
    {
        switch (currentInventory)
        {
            case 0:
                return inventoryItemsTreasure;
            case 1:
                return inventoryItemsResources;
            case 2:
                return inventoryItemsConsumables;
            case 3:
                return inventoryItemsEquipment;
            case 4:
                return inventoryItemsQuests;
        }
        return null;
    }
    
    public InventoryItem[] GetInventoryByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return inventoryItemsTreasure;
            case 1:
                return inventoryItemsResources;
            case 2:
                return inventoryItemsConsumables;
            case 3:
                return inventoryItemsEquipment;
            case 4:
                return inventoryItemsQuests;
        }
        return null;
    }

    private InventoryItem[] GetInventoryItemsByItemType(InventoryItem item)
    {
        switch (item.ItemType)
        {
            case ItemType.Treasure:
                return inventoryItemsTreasure;
            case ItemType.Resource:
                return inventoryItemsResources;
            case ItemType.Consumable:
                return inventoryItemsConsumables;
            case ItemType.Equipment:
                return inventoryItemsEquipment;
            case ItemType.Quest:
                return inventoryItemsQuests;
            default:
                return inventoryItemsTreasure;
        }
    }
    
    public void AddItem(InventoryItem item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            return;
        }

        InventoryItem[] targetInvetory = GetInventoryItemsByItemType(item);

        List<int> itemIndexes = CheckItemStockIndexes(item.ID);
        if (item.IsStackable && itemIndexes.Count > 0)
        {
            foreach (int index in itemIndexes)
            {
                int maxStack = item.MaxStack;
                if (targetInvetory[index].Quantity < maxStack)
                {
                    targetInvetory[index].Quantity += quantity;
                    if(targetInvetory[index].Quantity > maxStack)
                    {
                        int difference = targetInvetory[index].Quantity - maxStack;
                        targetInvetory[index].Quantity = maxStack;
                        AddItem(item, difference);  //recursive to fill up other stacks of same item 
                    }
                    UIManager.Instance.DrawItem(targetInvetory[index], index);
                    SaveInventory();
                    return;
                }
            }
        }

        // if what before ? is true take left side of : if false take right side
        int quantityToAdd = quantity > item.MaxStack ? item.MaxStack : quantity;    
        AddItemFreeSlot(item, quantityToAdd);
        int remainingAmount = quantity - quantityToAdd;
        if (remainingAmount > 0)
        {
            AddItem(item, remainingAmount);
        }
        
        SaveInventory();
    }

    public void RemoveItem(InventoryItem[] items, int index)
    {
        if (index >= items.Length) return;
        if (items[index] == null) return;
        if (items[index].ItemType == ItemType.Quest) return;
        
        items[index] = null;
        UIManager.Instance.DrawItem(null, index);
        audioManager.PlayRemoveItemSound();
        SaveInventory();
    }

    private void AddItemFreeSlot(InventoryItem item, int quantity)
    {
        InventoryItem[] items = GetInventoryItemsByItemType(item);
        
        for (int i = 0; i < inventorySize; i++)
        {
            if (items[i] != null)
            {
                continue;
            }
            items[i] = item.CopyItem();
            items[i].Quantity = quantity;
            UIManager.Instance.DrawItem(items[i], i);
            return;
        }
    }

    private void DecreaseItemStack(int index, InventoryItem item)
    {
        InventoryItem[] items = GetInventoryItemsByItemType(item);
        
        items[index].Quantity--;
        if (items[index].Quantity <= 0)
        {
            items[index] = null;
            UIManager.Instance.DrawItem(null, index);
        }
        else
        {
            UIManager.Instance.DrawItem(items[index], index);
        }
    }

    public void ConsumeItem(string itemID)
    {
        List<int> indexes = CheckItemStockIndexes(itemID);
        if (indexes.Count > 0)
        {
            // ^1 means the last one, so if there are 5 items in list it will be the 5th
            DecreaseItemStack(indexes[^1], ItemExistsInGameContent(itemID));
        }
    }

    private List<int> CheckItemStockIndexes(string itemID)
    {
        InventoryItem[] items = GetInventoryItemsByItemType(ItemExistsInGameContent(itemID));
        
        List<int> itemIndexes = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                continue;   
            }

            if (items[i].ID == itemID)
            {
                itemIndexes.Add(i);
            }
        }
        
        return itemIndexes;
    }

    public int GetItemCurrentStock(string itemID)
    {
        InventoryItem[] items = GetInventoryItemsByItemType(ItemExistsInGameContent(itemID));
        
        List<int> indexes = CheckItemStockIndexes(itemID);
        int currentStock = 0;
        foreach (int index in indexes)
        {
            if (items[index].ID == itemID)
            {
                currentStock += items[index].Quantity;
            }
        }
        return currentStock;
    }

    public InventoryItem ItemExistsInGameContent(string itemID)
    {
        for (int i = 0; i < gameContent.GameItems.Length; i++)
        {
            if (gameContent.GameItems[i].ID == itemID)
            {
                return gameContent.GameItems[i];
            }
        }
        
        return null;
    }

    #region Save, Load and Reset Inventory

    public void ResetInventory()
    {
        for (int index = 0; index < inventorySize; index++)
        {
            inventoryItemsTreasure[index] = null;
            inventoryItemsResources[index] = null;
            inventoryItemsConsumables[index] = null;
            inventoryItemsEquipment[index] = null;
            inventoryItemsQuests[index] = null;
            UIManager.Instance.DrawItem(null, index);
        }
        SaveInventory();
    }

    public void LoadInventory()
    {
        var inventories = new (InventoryItem[] array, string key)[]
        {
            (inventoryItemsTreasure, INVENTORY_TREASURE),
            (inventoryItemsResources, INVENTORY_RESOURCES),
            (inventoryItemsConsumables, INVENTORY_CONSUMABLES),
            (inventoryItemsEquipment, INVENTORY_EQUIPMENT),
            (inventoryItemsQuests, INVENTORY_QUESTS)
        };

        foreach (var (array, key) in inventories)
        {
            if (!SaveGame.Exists(key)) continue;

            InventoryData loadData = SaveGame.Load<InventoryData>(key);

            for (int i = 0; i < InventorySize; i++)
            {
                if (loadData.ItemContent[i] != null)
                {
                    InventoryItem itemFromContent = ItemExistsInGameContent(loadData.ItemContent[i]);
                    if (itemFromContent != null)
                    {
                        array[i] = itemFromContent.CopyItem();
                        array[i].Quantity = loadData.ItemQuantity[i];
                    }
                    else
                    {
                        array[i] = null;
                    }
                }
                else
                {
                    array[i] = null;
                }
            }
        }
    }
    
    public void SaveInventory()
    {
        var inventories = new (InventoryItem[] array, string key)[]
        {
            (inventoryItemsTreasure, INVENTORY_TREASURE),
            (inventoryItemsResources, INVENTORY_RESOURCES),
            (inventoryItemsConsumables, INVENTORY_CONSUMABLES),
            (inventoryItemsEquipment, INVENTORY_EQUIPMENT),
            (inventoryItemsQuests, INVENTORY_QUESTS)
        };
        
        foreach (var (array, key) in inventories)
        {
            InventoryData saveData = new InventoryData
            {
                ItemContent = new string[inventorySize],
                ItemQuantity = new int[inventorySize]
            };
            
            for (int i = 0; i < inventorySize; i++)
            {
                if (array[i] != null)
                {
                    saveData.ItemContent[i] = array[i].ID;
                    saveData.ItemQuantity[i] = array[i].Quantity;
                }
                else
                {
                    saveData.ItemContent[i] = null;
                    saveData.ItemQuantity[i] = 0;
                }
            }

            SaveGame.Save(key, saveData);
        }
    }
    
    #endregion
}
