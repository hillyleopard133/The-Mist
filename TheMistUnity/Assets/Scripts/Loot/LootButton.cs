using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootButton : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemQuantity;

    public DropItem ItemLoaded { get; private set; }

    public void ConfigLootButton(DropItem dropItem)
    {
        ItemLoaded = dropItem;
        itemIcon.sprite = dropItem.Item.Icon;
        itemName.text = dropItem.Item.Name;
        itemQuantity.text = $"x{dropItem.Quantity.ToString()}";
    }

    public void CollectItem()
    {
        if (ItemLoaded == null)
        {
            return;
        }
        Inventory.Instance.AddItem(ItemLoaded.Item, ItemLoaded.Quantity);
        ItemLoaded.PickedItem = true;
        if (LootManager.Instance.LootPanelItemCount() == 1)
        {
            LootManager.Instance.ClosePanel();
            Destroy(LootManager.Instance.CurrentEnemyLoot.gameObject);
        }
        Destroy(gameObject);
    }
}
