using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Singleton<Inventory>
{
    [Header("Config")] 
    [SerializeField] public GameContent gameContent;
    [SerializeField] private int initialInventorySize;
    [SerializeField] private InventoryItem[] inventoryItems;
    
    [Header("Filter")]
    [SerializeField] private TMP_Dropdown filterDropdown;

    public int InventorySize => inventorySize;
    private int inventorySize;
    public InventoryItem[] InventoryItems => inventoryItems;
    public InventoryItem[] FilteredItems => filteredInventory;
    
    private InventoryItem[] filteredInventory;

    private readonly string INVENTORY_KEY_DATA = "MY_INVENTORY";
    private readonly string EQUIPPED_WEAPON = "EQUIPPED_WEAPON";
    
    private AudioManager audioManager;

    protected override void Awake()
    {
        base.Awake();
        inventorySize = initialInventorySize;
    }

    private void Start()
    {
        inventoryItems = new InventoryItem[inventorySize];
        audioManager = AudioManager.Instance;
        VerifyItemsForDraw();
        LoadInventory();
        LoadEquippedWeapon();
        
        CreateFilterOptions();
        filterDropdown.onValueChanged.AddListener(UpdateInventoryFilter);
    }
    
    public void UpgradeInventory()
    {
        
    }

    private void CreateFilterOptions()
    {
        filterDropdown.ClearOptions();
        List<string> options = new List<string>(Enum.GetNames(typeof(ItemType)));
        options.Insert(0, "All");
        filterDropdown.AddOptions(options);
    }

    public bool IsInventoryFiltered()
    {
        return filteredInventory != null;
    }

    public void RemoveFilter()
    {
        filteredInventory = null;
        InventoryUI.Instance.DrawInventory(inventoryItems);
        filterDropdown.value = 0;
    }

    public void UpdateInventoryFilter(int index)
    {
        if (index == 0)
        {
            RemoveFilter();
            return;
        }
        
        ItemType itemType = (ItemType)(index - 1);
        filteredInventory = FilterInventory(itemType);
        InventoryUI.Instance.DrawInventory(filteredInventory);
    }

    private InventoryItem[] FilterInventory(ItemType itemType)
    {
        List<InventoryItem> newFilter = new List<InventoryItem>();
        foreach (InventoryItem item in inventoryItems)
        {
            if(item == null) continue;
            if (itemType == item.ItemType)
            {
                newFilter.Add(item);
            }
        }
        return newFilter.ToArray();
    }
    
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

    public void AddItem(InventoryItem item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            return;
        }

        List<int> itemIndexes = CheckItemStockIndexes(item.ID);
        if (item.IsStackable && itemIndexes.Count > 0)
        {
            foreach (int index in itemIndexes)
            {
                int maxStack = item.MaxStack;
                if (inventoryItems[index].Quantity < maxStack)
                {
                    inventoryItems[index].Quantity += quantity;
                    if(inventoryItems[index].Quantity > maxStack)
                    {
                        int difference = inventoryItems[index].Quantity - maxStack;
                        inventoryItems[index].Quantity = maxStack;
                        AddItem(item, difference);  //recursive to fill up other stacks of same item 
                    }
                    InventoryUI.Instance.DrawItem(inventoryItems[index], index);
                    SaveInventory();
                    GetComponent<Hotbar>().UpdateUI();
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
        GetComponent<Hotbar>().UpdateUI();
    }

    public void UseItem(int index)
    {
        InventoryItem[] items = inventoryItems;
        if (filteredInventory != null) items = filteredInventory;
        if (index >= items.Length) return;
        
        if (items[index] == null) return;

        if (items[index].UseItem())
        {
            DecreaseItemStack(index);
            audioManager.PlayUseItemSound();
        }
        GetComponent<Hotbar>().UpdateUI();
        SaveInventory();
    }

    public void RemoveItem(int index)
    {
        InventoryItem[] items = inventoryItems;
        if (filteredInventory != null) items = filteredInventory;
        if (index >= items.Length) return;
        
        if (items[index] == null) return;
        if (items[index].ItemType == ItemType.Quest) return;
        
        
        RemoveFromInventory(items[index]);
        items[index] = null;
        InventoryUI.Instance.DrawItem(null, index);
        audioManager.PlayRemoveItemSound();
        GetComponent<Hotbar>().UpdateUI();
        SaveInventory();
    }

    public void RemoveFromInventory(InventoryItem item)
    {
        List<InventoryItem> items = inventoryItems.ToList();
        if (items.Contains(item))
        {
            int index = items.IndexOf(item);
            inventoryItems[index] = null;
        }
    }

    public void EquipItem(int index)
    {
        InventoryItem[] items = inventoryItems;
        if (filteredInventory != null) items = filteredInventory;
        if (index >= items.Length) return;
        
        if (items[index] == null) return;
        if (items[index].ItemType != ItemType.Weapon) return;
        
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

    private void AddItemFreeSlot(InventoryItem item, int quantity)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] != null)
            {
                continue;
            }
            inventoryItems[i] = item.CopyItem();
            inventoryItems[i].Quantity = quantity;
            InventoryUI.Instance.DrawItem(inventoryItems[i], i);
            return;
        }
    }

    private void DecreaseItemStack(int index)
    {
        InventoryItem[] items = inventoryItems;
        if (filteredInventory != null) items = filteredInventory;
        
        items[index].Quantity--;
        if (items[index].Quantity <= 0)
        {
            items[index] = null;
            InventoryUI.Instance.DrawItem(null, index);
        }
        else
        {
            InventoryUI.Instance.DrawItem(items[index], index);
        }
    }

    public void ConsumeItem(string itemID)
    {
        List<int> indexes = CheckItemStockIndexes(itemID);
        if (indexes.Count > 0)
        {
            // ^1 means the last one, so if there are 5 items in list it will be the 5th
            DecreaseItemStack(indexes[^1]);
        }
    }

    public List<int> CheckItemStockIndexes(string itemID)
    {
        List<int> itemIndexes = new List<int>();
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                continue;   //like return but for for loops to move onto the next iteration, skips rest of code in the loop
            }

            if (inventoryItems[i].ID == itemID)
            {
                itemIndexes.Add(i);
            }
        }
        
        return itemIndexes;
    }

    public int GetItemCurrentStock(string itemID)
    {
        List<int> indexes = CheckItemStockIndexes(itemID);
        int currentStock = 0;
        foreach (int index in indexes)
        {
            if (inventoryItems[index].ID == itemID)
            {
                currentStock += inventoryItems[index].Quantity;
            }
        }
        return currentStock;
    }

    private void VerifyItemsForDraw()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] == null)
            {
                InventoryUI.Instance.DrawItem(null, i);
            }
        }
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

    public void ResetInventory()
    {
        for (int index = 0; index < inventorySize; index++)
        {
            inventoryItems[index] = null;
            InventoryUI.Instance.DrawItem(null, index);
        }
        GetComponent<Hotbar>().UpdateUI();
        SaveInventory();
    }

    private void LoadInventory()
    {
        if (SaveGame.Exists(INVENTORY_KEY_DATA))
        {
            InventoryData loadData = SaveGame.Load<InventoryData>(INVENTORY_KEY_DATA);
            for (int i = 0; i < InventorySize; i++)
            {
                if (loadData.ItemContent[i] != null)
                {
                    InventoryItem itemFromContent = ItemExistsInGameContent(loadData.ItemContent[i]);
                    if (itemFromContent != null)
                    {
                        inventoryItems[i] = itemFromContent.CopyItem();
                        inventoryItems[i].Quantity = loadData.ItemQuantity[i];
                        InventoryUI.Instance.DrawItem(inventoryItems[i], i);
                    }
                }
                else
                {
                    inventoryItems[i] = null;
                }
            }
        }
    }

    private void SaveInventory()
    {
        InventoryData saveData = new InventoryData();
        saveData.ItemContent = new string[inventorySize];
        saveData.ItemQuantity = new int[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] == null)
            {
                saveData.ItemContent[i] = null;
                saveData.ItemQuantity[i] = 0;
            }
            else
            {
                saveData.ItemContent[i] = inventoryItems[i].ID;
                saveData.ItemQuantity[i] = inventoryItems[i].Quantity;
            }
            //Using the imported package save gold
            SaveGame.Save(INVENTORY_KEY_DATA, saveData);
        }
    }
    
}
