using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [HideInInspector] public int shopItemAmount = 1;
    private InventoryItem selectedItem;
    private bool isBuying;
    private List<int> treasureIndexes = new List<int>();
    private Inventory inventory;
    private CoinManager coinManager;
    private AudioManager audioManager;
    private UIManager uIManager;

    private void Start()
    {
        inventory = Inventory.Instance;
        coinManager = CoinManager.Instance;
        audioManager = AudioManager.Instance;
        uIManager = UIManager.Instance;
    }
    
    public int CalculatePrice(bool isBuying = true)
    {
        if(isBuying) return selectedItem.BuyValue * shopItemAmount;
        return selectedItem.SellValue * shopItemAmount;
    }

    public int CalculateAllTreasureValue()
    {
        InventoryItem[] treasures = inventory.GetInventoryByIndex(0);
        int treasuresValue = 0;
        treasureIndexes.Clear();
        
        for(int i = 0; i < treasures.Length; i++)
        {
            if (treasures[i] != null)
            {
                treasuresValue += treasures[i].SellValue * treasures[i].Quantity;
                treasureIndexes.Add(i);
            }
        }
        
        return treasuresValue;
    }

    public void SelectItem(InventoryItem item, bool buying)
    {
        if(selectedItem != item) shopItemAmount = 1;
        selectedItem = item;
        isBuying = buying;
    }

    public void BuySellItem()
    {
        if(isBuying) BuyItem();
        else SellItem();
    }

    private void PressButton()
    {
        audioManager.PlayButtonPressSound();
        uIManager.RefreshShop();
    }
    
    private void BuyItem()
    {
        if (coinManager.Coins >= CalculatePrice())
        {
            inventory.AddItem(selectedItem, shopItemAmount);
            coinManager.RemoveCoins(CalculatePrice());
            PressButton();
        }
    }

    private void SellItem()
    {
        coinManager.AddCoins(CalculatePrice());
        for (int i = 0; i < shopItemAmount; i++)
        {
            inventory.ConsumeItem(selectedItem.ID);
        }
        PressButton();
    }

    public void SellAllTreasure()
    {
        coinManager.AddCoins(CalculateAllTreasureValue());
        InventoryItem[] treasures = inventory.GetInventoryByIndex(0);
        foreach (int index in treasureIndexes)
        {
            inventory.RemoveItem(treasures, index);
        }
        PressButton();
    }

    public void IncreaseItemAmount()
    {
        shopItemAmount++;
        PressButton();
    }

    public void DecreaseItemAmount()
    {
        shopItemAmount--;
        PressButton();
    }

    public void SetItemAmountToMax()
    {
        if (isBuying)
        {
            int maxAffordable = coinManager.Coins / selectedItem.BuyValue;
            if (maxAffordable >= selectedItem.MaxStack)
            {
                shopItemAmount = selectedItem.MaxStack;
            }
            else
            {
                shopItemAmount = maxAffordable;
            }
        }
        else
        {
            int itemAmount = inventory.GetItemCurrentStock(selectedItem.ID);
            if (itemAmount >= selectedItem.MaxStack)
            {
                shopItemAmount = selectedItem.MaxStack;
            }
            else
            {
                shopItemAmount = itemAmount;
            }
        }
        PressButton();
    }

    public void SetItemAmountToMin()
    {
        shopItemAmount = 1;
        PressButton();
    }
}