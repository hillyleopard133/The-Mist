using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image slot1;
    [SerializeField] private Image slot2;
    [SerializeField] private Image slot3;
    [SerializeField] private TextMeshProUGUI slot1Amount;
    [SerializeField] private TextMeshProUGUI slot2Amount;
    [SerializeField] private TextMeshProUGUI slot3Amount;

    private int hotbarSize = 3;
    
    private PlayerActions actions;

    [HideInInspector] public InventoryItem[] Items { get; set; }
    
    private InventoryItem selectedItem;
    
    private readonly string HOTBAR = "HOTBAR";

    private void Start()
    {
        actions = new PlayerActions();  
        actions.Enable();
        Items = new InventoryItem[hotbarSize];
        actions.Hotbar.Slot1.performed += ctx => UseSlot1();
        actions.Hotbar.Slot2.performed += ctx => UseSlot2();
        actions.Hotbar.Slot3.performed += ctx => UseSlot3();
    }

    public void LoadHotbarItems()
    {
        if (SaveGame.Exists(HOTBAR))
        {
            int[] indexes = SaveGame.Load<int[]>(HOTBAR);
            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] == -1)
                {
                    Items[i] = null;
                    continue;
                }
                Items[i] = Inventory.Instance.InventoryItems[indexes[i]];
            }
            UpdateUI();
        }
    }

    public void SaveHotbarItems()
    {
        int[] indexes = new int[Items.Length];
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] != null)
            {
                indexes[i] = Inventory.Instance.CheckItemStockIndexes(Items[i].ID)[0];
            }
            else
            {
                indexes[i] = -1;
            }
        }
        SaveGame.Save(HOTBAR, indexes);
    }

    public void ResetHotbarItems()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = null;
        }
        UpdateUI();
        SaveHotbarItems();
    }

    public void UpdateUI()
    {
        if (Items[0] != null)
        {
            slot1.gameObject.SetActive(true);
            slot1Amount.gameObject.SetActive(true);
            slot1.sprite = Items[0].Icon;
            int amount = Inventory.Instance.GetItemCurrentStock(Items[0].ID);
            slot1Amount.text = amount.ToString();
            
        }
        else
        {
            slot1.gameObject.SetActive(false);
            slot1Amount.gameObject.SetActive(false);
        }
        if (Items[1] != null)
        {
            slot2.gameObject.SetActive(true);
            slot2Amount.gameObject.SetActive(true);
            slot2.sprite = Items[1].Icon;
            int amount = Inventory.Instance.GetItemCurrentStock(Items[1].ID);
            slot2Amount.text = amount.ToString();
            
        }
        else
        {
            slot2.gameObject.SetActive(false);
            slot2Amount.gameObject.SetActive(false);
        }
        if (Items[2] != null)
        {
            slot3.gameObject.SetActive(true);
            slot3Amount.gameObject.SetActive(true);
            slot3.sprite = Items[2].Icon;
            int amount = Inventory.Instance.GetItemCurrentStock(Items[2].ID);
            slot3Amount.text = amount.ToString();
            
        }
        else
        {
            slot3.gameObject.SetActive(false);
            slot3Amount.gameObject.SetActive(false);
        }
    }

    private void UseSlot1()
    {
        if (InventoryUI.Instance.IsInventoryOpen())
        {
            EquipItemIntoHotbar(0);
        }
        else
        {
            if (Items[0] != null)
            {
                UseItem(0);
            }
        }
    }

    private void UseSlot2()
    {        
        if (InventoryUI.Instance.IsInventoryOpen())
        {
            EquipItemIntoHotbar(1);
        }
        else
        {
            if (Items[1] != null)
            {
                UseItem(1);
            }
        }
    }

    private void UseSlot3()
    {
        if (InventoryUI.Instance.IsInventoryOpen())
        {
            EquipItemIntoHotbar(2);
        }
        else
        {
            if (Items[2] != null)
            {
                UseItem(2);
            }
        }
    }

    private void EquipItemIntoHotbar(int index)
    {
        if (selectedItem == null)
        {
            Items[index] = null;
        }
        else if (selectedItem.IsConsumable)
        {
            Items[index] = selectedItem;
        }
        SaveHotbarItems();
        UpdateUI();
    }

    private void UseItem(int index)
    {
        StartCoroutine(UseItemCoroutine(index));
    }

    private IEnumerator UseItemCoroutine(int index)
    {
        List<int> indexes = Inventory.Instance.CheckItemStockIndexes(Items[index].ID);
        
        yield return null;
        
        Inventory.Instance.UseItem(indexes[^1]);
        
        yield return null;
        
        if (Inventory.Instance.GetItemCurrentStock(Items[index].ID) <= 0)
        {
            Items[index] = null;
        }
        
        yield return null;
        
        UpdateUI();
    }

    private void SlotSelectedCallback(int slotIndex)
    {
        selectedItem = Inventory.Instance.InventoryItems[slotIndex];
        
    }
    
    private void OnEnable()
    {
        InventorySlot.OnSlotSelectedEvent += SlotSelectedCallback;
    }

}