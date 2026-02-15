using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoLootItem : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemQuantity;
    [SerializeField] private TextMeshProUGUI itemDropChance;

    public DropItem ItemLoaded { get; private set; }

    public void ConfigLootInfo(DropItem dropItem)
    {
        ItemLoaded = dropItem;
        itemIcon.sprite = dropItem.Item.Icon;
        itemName.text = dropItem.Item.Name;
        itemQuantity.text = dropItem.Quantity.ToString();
        itemDropChance.text = dropItem.DropChance.ToString();
    }
}