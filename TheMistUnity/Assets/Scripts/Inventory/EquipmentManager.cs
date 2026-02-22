using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree;
using UnityEngine;

public class EquipmentManager : Singleton<EquipmentManager>
{
    private ItemArmour[] equippedArmour;
    private ItemWeapon[] equippedWeapons;
    private ItemScroll[] equippedScrolls;
    
    private const string EQUIPMENT_ARMOUR = "EQUIPMENT_ARMOUR";
    private const string EQUIPMENT_WEAPONS = "EQUIPMENT_WEAPONS";
    private const string EQUIPMENT_SCROLL = "EQUIPMENT_SCROLL";
    
    private const string EQUIPMENT_EQUIPPED = "EQUIPMENT_EQUIPPED";
    
    private int[] armourIndexes, weaponIndexes, scrollIndexes;
    private int[] equippedArmourSlotIndexes, equippedWeaponSlotIndexes, equippedScrollSlotIndexes;

    private int currentType;
    
    [SerializeField] public int inventorySize = 9;
    [HideInInspector] public PartyMember[] partyMembers;
    private int partySize;

    private void Start()
    {
        partyMembers = SkillsManager.Instance.partyMembers;
        partySize = partyMembers.Length;
        
        equippedArmour = new ItemArmour[partySize];
        equippedWeapons = new ItemWeapon[partySize];
        equippedScrolls = new ItemScroll[partySize];

        armourIndexes = new int[inventorySize]; 
        weaponIndexes = new int[inventorySize];
        scrollIndexes = new int[inventorySize];
        
        equippedArmourSlotIndexes = new int[partySize];
        equippedWeaponSlotIndexes = new int[partySize];
        equippedScrollSlotIndexes = new int[partySize];
        
        LoadEquipment();
    }

    public InventoryItem[] GetCharacterEquipment(int characterIndex)
    {
        InventoryItem[] equippedItems = new InventoryItem[3];
        equippedItems[0] = equippedArmour[characterIndex];
        equippedItems[1] = equippedWeapons[characterIndex];
        equippedItems[2] = equippedScrolls[characterIndex];
        return equippedItems;
    }

    public int GetEquippedSlotIndex(int equipmentType, int characterIndex)
    {
        switch (equipmentType)
        {
            case 0:
                return equippedArmourSlotIndexes[characterIndex];
            case 1:
                return equippedWeaponSlotIndexes[characterIndex];
            case 2:
                return equippedScrollSlotIndexes[characterIndex];
        }
        
        return -1;
    }

    public void EquipItem(int slotIndex, int characterIndex)
    {
        switch (currentType)
        {
            case 0:
                UnequipItem(equippedArmourSlotIndexes[characterIndex], characterIndex);
                ItemArmour itemArmour = (ItemArmour) Inventory.Instance.InventoryItemsEquipment[armourIndexes[slotIndex]];
                equippedArmour[characterIndex] = itemArmour;
                itemArmour.equipped = characterIndex;
                partyMembers[characterIndex].EquipItem(itemArmour);
                break;
            case 1:
                UnequipItem(equippedWeaponSlotIndexes[characterIndex], characterIndex);
                ItemWeapon itemWeapon = (ItemWeapon) Inventory.Instance.InventoryItemsEquipment[weaponIndexes[slotIndex]];
                equippedWeapons[characterIndex] = itemWeapon;
                itemWeapon.equipped = characterIndex;
                partyMembers[characterIndex].EquipItem(itemWeapon);
                break;
            case 2:
                UnequipItem(equippedScrollSlotIndexes[characterIndex], characterIndex);
                ItemScroll itemScroll = (ItemScroll) Inventory.Instance.InventoryItemsEquipment[scrollIndexes[slotIndex]];
                equippedScrolls[characterIndex] = itemScroll;
                itemScroll.equipped = characterIndex;
                partyMembers[characterIndex].EquipItem(itemScroll);
                break;
        }
        SaveEquipment();
    }

    public void UnequipItem(int slotIndex, int characterIndex)
    {
        if (slotIndex == -1) return;
        
        switch (currentType)
        {
            case 0:
                ItemArmour itemArmour = (ItemArmour) Inventory.Instance.InventoryItemsEquipment[armourIndexes[slotIndex]];
                equippedArmour[characterIndex] = null; 
                itemArmour.equipped = -1; 
                partyMembers[characterIndex].UnEquipItem(itemArmour);
                break;
            case 1:
                ItemWeapon itemWeapon = (ItemWeapon) Inventory.Instance.InventoryItemsEquipment[weaponIndexes[slotIndex]];
                equippedWeapons[characterIndex] = null;
                itemWeapon.equipped = -1;
                partyMembers[characterIndex].UnEquipItem(itemWeapon);
                break;
            case 2:
                ItemScroll itemScroll = (ItemScroll) Inventory.Instance.InventoryItemsEquipment[scrollIndexes[slotIndex]];
                equippedScrolls[characterIndex] = null;
                itemScroll.equipped = -1;
                partyMembers[characterIndex].UnEquipItem(itemScroll);
                break;
        }
        SaveEquipment();
    }

    public ItemEquipment[] SortEquipment(int itemType)
    {
        ItemEquipment[] items = Inventory.Instance.InventoryItemsEquipment;
        
        ItemEquipment[] weapons = new ItemEquipment[inventorySize];
        ItemEquipment[] armours = new ItemEquipment[inventorySize];
        ItemEquipment[] scrolls = new ItemEquipment[inventorySize];

        int weaponIndex = 0;
        int armourIndex = 0;
        int scrollIndex = 0;
        
        for (int i = 0; i < partySize; i++)
        {
            equippedWeaponSlotIndexes[i] = -1;
            equippedArmourSlotIndexes[i] = -1;
            equippedScrollSlotIndexes[i] = -1;
        }

        for (int i = 0; i < items.Length; i++)
        {
            ItemEquipment item = items[i];
            switch (item)
            {
                case ItemWeapon weapon:
                    weaponIndexes[weaponIndex] = i;
                    if (weapon.equipped != -1) equippedWeaponSlotIndexes[weapon.equipped] = weaponIndex;
                    if (weaponIndex < inventorySize) weapons[weaponIndex++] = weapon;
                    break;

                case ItemArmour armour:
                    armourIndexes[armourIndex] = i;
                    if(armour.equipped != -1) equippedArmourSlotIndexes[armour.equipped] = armourIndex;
                    if (armourIndex < inventorySize) armours[armourIndex++] = armour;
                    break;

                case ItemScroll scroll:
                    scrollIndexes[scrollIndex] = i;
                    if (scroll.equipped != -1) equippedScrollSlotIndexes[scroll.equipped] = scrollIndex;
                    if (scrollIndex < inventorySize) scrolls[scrollIndex++] = scroll;
                    break;
            }
        }

        switch (itemType)
        {
            case 0:
                currentType = 0;
                return armours;
            case 1:
                currentType = 1;
                return weapons;
            case 2:
                currentType = 2;
                return scrolls;
            default:
                return null;
        }
    }

    #region Save, Load and Reset Equipment

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
        
        ItemEquipment[] inventoryItemsEquipment = Inventory.Instance.InventoryItemsEquipment;
                
        if (SaveGame.Exists(EQUIPMENT_EQUIPPED))
        {
            int[] equippedItems = SaveGame.Load<int[]>(EQUIPMENT_EQUIPPED);

            for (int i = 0; i < Inventory.Instance.InventorySize; i++)
            {
                if (inventoryItemsEquipment[i] != null)
                {
                    inventoryItemsEquipment[i].equipped = equippedItems[i];
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

        ItemEquipment[] inventoryItemsEquipment = Inventory.Instance.InventoryItemsEquipment;
        int[] equippedItems = new int[Inventory.Instance.InventorySize];
        
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            if (inventoryItemsEquipment[i] != null)
            {
                equippedItems[i] = inventoryItemsEquipment[i].equipped;
            }
            else
            {
                equippedItems[i] = -1;
            }
        }
        
        SaveGame.Save(EQUIPMENT_EQUIPPED, equippedItems);
    }
    
    #endregion
}
