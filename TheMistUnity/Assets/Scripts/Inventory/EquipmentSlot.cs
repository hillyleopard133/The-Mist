using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public static event Action<int> OnSlotSelectedEvent; 
    
    [SerializeField] private Image itemIcon;
    
    public int Index {get; set;}

    public void ClickSlot()
    {
        OnSlotSelectedEvent?.Invoke(Index);
    }

    public void UpdateSlot(InventoryItem item)
    {
        itemIcon.sprite = item.Icon;
    }

    public void ShowSlotInformation(bool value)
    {
        itemIcon.gameObject.SetActive(value);
    }
}