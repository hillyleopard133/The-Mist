using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public static event Action<int> OnSlotSelectedEvent; 
    
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite slotNormal;
    [SerializeField] private Sprite slotSelected;
    [SerializeField] private Image equippedCharacterIcon;
    
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

    public void UpdateSlot(InventoryItem item, Sprite icon)
    {
        itemIcon.sprite = item.Icon;
        
        if (icon != null)
        {
            equippedCharacterIcon.gameObject.SetActive(true);
            equippedCharacterIcon.sprite = icon;
        }
        else
        {
            equippedCharacterIcon.gameObject.SetActive(false);
        }
    }

    public void ShowSlotInformation(bool value)
    {
        itemIcon.gameObject.SetActive(value);
        equippedCharacterIcon.gameObject.SetActive(value);
    }
}