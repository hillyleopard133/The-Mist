using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public class EquipmentManager : Singleton<EquipmentManager>
{
    private ItemWeapon[] equippedWeapons;
    private ItemArmour[] equippedArmour;
    private ItemScroll[] equippedScrolls;

    private const int partySize = 3;
    
    private const string EQUIPMENT_WEAPONS = "EQUIPMENT_WEAPONS";
    private const string EQUIPMENT_ARMOUR = "EQUIPMENT_ARMOUR";
    private const string EQUIPMENT_SCROLL = "EQUIPMENT_SCROLL";
    
    [SerializeField] public int inventorySize = 9;

    private void Start()
    {
        equippedWeapons = new ItemWeapon[partySize];
        equippedArmour = new ItemArmour[partySize];
        equippedScrolls = new ItemScroll[partySize];
        
        LoadEquipment();
    }

    public InventoryItem[] SortEquipment(int index)
    {
        InventoryItem[] items = Inventory.Instance.InventoryItemsEquipment;
        
        InventoryItem[] weapons = new InventoryItem[inventorySize];
        InventoryItem[] armours = new InventoryItem[inventorySize];
        InventoryItem[] scrolls = new InventoryItem[inventorySize];

        int weaponIndex = 0;
        int armourIndex = 0;
        int scrollIndex = 0;

        foreach (InventoryItem item in items)
        {
            if (item is ItemWeapon)
            {
                if (weaponIndex < inventorySize)
                    weapons[weaponIndex++] = item;
            }
            else if (item is ItemArmour)
            {
                if (armourIndex < inventorySize)
                    armours[armourIndex++] = item;
            }
            else if (item is ItemScroll)
            {
                if (scrollIndex < inventorySize)
                    scrolls[scrollIndex++] = item;
            }
        }

        switch (index)
        {
            case 0:
                return armours;
            case 1:
                return weapons;
            case 2:
                return scrolls;
            default:
                return null;
        }
    }

    public void ResetEquipment()
    {
        for (int i = 0; i < partySize; ++i)
        {
            equippedWeapons[i] = null;
            equippedArmour[i] = null;
            equippedScrolls[i] = null;
        }
        
        SaveEquipment();
    }

    public void LoadEquipment()
    {
        if (SaveGame.Exists(EQUIPMENT_WEAPONS))
        {
            InventoryData weapons = SaveGame.Load<InventoryData>(EQUIPMENT_WEAPONS);

            for (int i = 0; i < partySize; i++)
            {
                if (weapons.ItemContent[i] != null)
                {
                    InventoryItem item = Inventory.Instance.ItemExistsInGameContent(weapons.ItemContent[i]);
                    if (item != null)
                    {
                        equippedWeapons[i] = (ItemWeapon)item.CopyItem();
                        equippedWeapons[i].Quantity = weapons.ItemQuantity[i];
                    }
                    else
                    {
                        equippedWeapons[i] = null;
                    }
                }
                else
                {
                    equippedWeapons[i] = null;
                }
            }
        }

        if (SaveGame.Exists(EQUIPMENT_ARMOUR))
        {
            InventoryData armour = SaveGame.Load<InventoryData>(EQUIPMENT_ARMOUR);

            for (int i = 0; i < partySize; i++)
            {
                if (armour.ItemContent[i] != null)
                {
                    InventoryItem item = Inventory.Instance.ItemExistsInGameContent(armour.ItemContent[i]);

                    if (item != null)
                    {
                        equippedArmour[i] = (ItemArmour)item.CopyItem();
                        equippedArmour[i].Quantity = armour.ItemQuantity[i];
                    }
                    else
                    {
                        equippedArmour[i] = null;
                    }
                }
                else
                {
                    equippedArmour[i] = null;
                }
            }
        }

        if (SaveGame.Exists(EQUIPMENT_SCROLL))
        {
            InventoryData scrolls = SaveGame.Load<InventoryData>(EQUIPMENT_SCROLL);

            for (int i = 0; i < partySize; i++)
            {
                if (scrolls.ItemContent[i] != null)
                {
                    InventoryItem item = Inventory.Instance.ItemExistsInGameContent(scrolls.ItemContent[i]);

                    if (item != null)
                    {
                        equippedScrolls[i] = (ItemScroll)item.CopyItem();
                        equippedScrolls[i].Quantity = scrolls.ItemQuantity[i];
                    }
                    else
                    {
                        equippedScrolls[i] = null;
                    }
                }
                else
                {
                    equippedScrolls[i] = null;
                }
            }
        }
    }

    public void SaveEquipment()
    {
        InventoryData weapons = new InventoryData
        {
            ItemContent = new string[partySize],
            ItemQuantity = new int[partySize]
        };
        
        InventoryData armour = new InventoryData
        {
            ItemContent = new string[partySize],
            ItemQuantity = new int[partySize]
        };
        
        InventoryData scrolls = new InventoryData
        {
            ItemContent = new string[partySize],
            ItemQuantity = new int[partySize]
        };
        
        for (int i = 0; i < partySize; i++)
        {
            if (equippedWeapons[i] != null)
            {
                weapons.ItemContent[i] = equippedWeapons[i].ID;
                weapons.ItemQuantity[i] = equippedWeapons[i].Quantity;
            }
            else
            {
                weapons.ItemContent[i] = null;
                weapons.ItemQuantity[i] = 0;
            }
            
            if (equippedArmour[i] != null)
            {
                armour.ItemContent[i] = equippedArmour[i].ID;
                armour.ItemQuantity[i] = equippedArmour[i].Quantity;
            }
            else
            {
                armour.ItemContent[i] = null;
                armour.ItemQuantity[i] = 0;
            }
            
            if (equippedScrolls[i] != null)
            {
                scrolls.ItemContent[i] = equippedScrolls[i].ID;
                scrolls.ItemQuantity[i] = equippedScrolls[i].Quantity;
            }
            else
            {
                scrolls.ItemContent[i] = null;
                scrolls.ItemQuantity[i] = 0;
            }
        }
        
        SaveGame.Save(EQUIPMENT_WEAPONS, weapons);
        SaveGame.Save(EQUIPMENT_ARMOUR, armour);
        SaveGame.Save(EQUIPMENT_SCROLL, scrolls);
    }
}
