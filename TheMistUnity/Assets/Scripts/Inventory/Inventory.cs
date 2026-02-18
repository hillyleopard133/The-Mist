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
    [SerializeField] private int initialInventorySize;
    [SerializeField] private InventoryItem[] inventoryItemsTreasure;
    [SerializeField] private InventoryItem[] inventoryItemsResources;
    [SerializeField] private InventoryItem[] inventoryItemsConsumables;
    [SerializeField] private InventoryItem[] inventoryItemsEquipment;
    [SerializeField] private InventoryItem[] inventoryItemsQuests;

    public int InventorySize => inventorySize;
    private int inventorySize;
    public InventoryItem[] InventoryItemsTreasure => inventoryItemsTreasure;
    public InventoryItem[] InventoryItemsResources => inventoryItemsResources;
    public InventoryItem[] InventoryItemsConsumables => inventoryItemsConsumables;
    public InventoryItem[] InventoryItemsEquipment => inventoryItemsEquipment;
    public InventoryItem[] InventoryItemsQuests => inventoryItemsQuests;
    
    private const string INVENTORY_TREASURE = "Inventory_Treasure";
    private const string INVENTORY_RESOURCES = "Inventory_Resources";
    private const string INVENTORY_CONSUMABLES = "Inventory_Consumables";
    private const string INVENTORY_EQUIPMENT = "Inventory_Equipment";
    private const string INVENTORY_QUESTS = "Inventory_Quests";
    
    private readonly string EQUIPPED_WEAPON = "EQUIPPED_WEAPON";
    
    private AudioManager audioManager;

    private int currentInventory;
    
    protected override void Awake()
    {
        base.Awake();
        inventorySize = initialInventorySize;
    }

    private void Start()
    {
        inventoryItemsTreasure = new InventoryItem[inventorySize];
        inventoryItemsResources = new InventoryItem[inventorySize];
        inventoryItemsConsumables = new InventoryItem[inventorySize];
        inventoryItemsEquipment = new InventoryItem[inventorySize];
        inventoryItemsQuests = new InventoryItem[inventorySize];
        
        audioManager = AudioManager.Instance;
        LoadInventory();
        //LoadEquippedWeapon();
        
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
    
    public void UpgradeInventory()
    {
        
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

    public void RemoveItem(int index)
    {
        InventoryItem[] items = GetCurrentInventory();
        if (index >= items.Length) return;
        if (items[index] == null) return;
        if (items[index].ItemType == ItemType.Quest) return;
        
        RemoveFromInventory(items[index]);
        items[index] = null;
        UIManager.Instance.DrawItem(null, index);
        audioManager.PlayRemoveItemSound();
        SaveInventory();
    }

    public void RemoveFromInventory(InventoryItem item)
    {
        List<InventoryItem> items = GetCurrentInventory().ToList();
        if (items.Contains(item))
        {
            int index = items.IndexOf(item);
            GetCurrentInventory()[index] = null;
        }
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

    public List<int> CheckItemStockIndexes(string itemID)
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

    private InventoryItem ItemExistsInGameContent(string itemID)
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
    
    /*
public void EquipItem(int index)
{
    InventoryItem[] items = inventoryItemsTreasure;
    if (filteredInventory != null) items = filteredInventory;
    if (index >= items.Length) return;

    if (items[index] == null) return;
    //if (items[index].ItemType != ItemType.Weapon) return;

    InventoryItem item = items[index];
    RemoveFromInventory(items[index]);
    items[index] = null;
    InventoryUI.Instance.DrawItem(null, index);
    AddItem(FindWeaponInventoryItem(Player.Instance.gameObject.GetComponent<PlayerAttack>().CurrentWeapon),1);
    item.EquipItem();
    audioManager.PlayEquipItemSound();
    SaveEquippedWeapon();
    if(filteredInventory != null) UpdateInventoryFilter(filterDropdown.value);
}
*/
    
    /*
private ItemWeapon FindWeaponInventoryItem(Weapon weapon)
{
    foreach (InventoryItem item in gameContent.GameItems)
    {
        ItemWeapon weaponItem = item as ItemWeapon;
        if (weaponItem != null && weaponItem.Icon == weapon.Icon)
        {
            return weaponItem;
        }
    }
    return null;
}

public void SaveEquippedWeapon()
{
    ItemWeapon itemWeapon = FindWeaponInventoryItem(Player.Instance.gameObject.GetComponent<PlayerAttack>().CurrentWeapon);
    string itemID = itemWeapon.ID;
    SaveGame.Save(EQUIPPED_WEAPON, itemID);
}

public void LoadEquippedWeapon()
{
    if (SaveGame.Exists(EQUIPPED_WEAPON))
    {
        string itemID = SaveGame.Load<string>(EQUIPPED_WEAPON);
        InventoryItem itemFromContent = Inventory.Instance.ItemExistsInGameContent(itemID);
        ItemWeapon weaponItem = itemFromContent as ItemWeapon;
        if (weaponItem != null)
        {
            WeaponManager.Instance.EquipWeapon(weaponItem.Weapon);
            Player.Instance.PlayerAttack.EquipWeapon(weaponItem.Weapon);
        }
    }
}

*/

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

    private void LoadInventory()
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
    
    private void SaveInventory()
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

    
}
