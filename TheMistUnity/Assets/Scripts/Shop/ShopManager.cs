using System;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [Header("Config")]
    [SerializeField] private ShopCard shopCardPrefab;
    [SerializeField] private Transform shopContainer;
    
    [HideInInspector] public ShopItem[] Items { get; set; }

    public void LoadShop()
    {
        if (Items == null) Debug.Log("ShopItems null");
        foreach (Transform child in shopContainer)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < Items.Length; i++)
        {
            ShopCard card = Instantiate(shopCardPrefab, shopContainer);
            card.ConfigShopCard(Items[i]);
        }
    }
    
}