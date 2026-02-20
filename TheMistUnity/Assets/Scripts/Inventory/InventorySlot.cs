using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public static event Action<int> OnSlotSelectedEvent; 
    
    [Header("Config")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image quantityContainer;
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite slotNormal;
    [SerializeField] private Sprite slotSelected;
    [SerializeField] private TextMeshProUGUI itemQuantityTMP;
    
    public int Index {get; set;}

    public void ClickSlot()
    {
        OnSlotSelectedEvent?.Invoke(Index);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            slotImage.sprite = slotSelected;
        }
        else
        {
            slotImage.sprite = slotNormal;
        }
    }

    public void UpdateSlot(InventoryItem item)
    {
        itemIcon.sprite = item.Icon;
        itemQuantityTMP.text = item.Quantity.ToString();
    }

    public void ShowSlotInformation(bool value)
    {
        itemIcon.gameObject.SetActive(value);
        quantityContainer.gameObject.SetActive(value);
    }
}