using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, ISelectHandler
{
    public static event Action<int> OnSlotSelectedEvent; 
    
    [Header("Config")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image quantityContainer;
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite slotNormal;
    [SerializeField] private Sprite slotSelected;
    [SerializeField] private TextMeshProUGUI itemQuantityTMP;
    [SerializeField] private Image equippedCharacterIcon;
    
    public int Index {get; set;}

    public void ClickSlot()
    {
        OnSlotSelectedEvent?.Invoke(Index);
    }
    
    public void OnSelect(BaseEventData eventData)
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

    public void UpdateSlot(InventoryItem item, bool isNPCShopItem = false)
    {
        if (item is ItemEquipment equipment)
        {
            quantityContainer.gameObject.SetActive(false);
            if (equipment.equipped != -1)
            {
                equippedCharacterIcon.gameObject.SetActive(true);
                equippedCharacterIcon.sprite = UIManager.Instance.characterIcons[equipment.equipped];
            }
            else
            {
                equippedCharacterIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            equippedCharacterIcon.gameObject.SetActive(false);
            quantityContainer.gameObject.SetActive(true);
            itemQuantityTMP.text = item.Quantity.ToString();
        }
        itemIcon.sprite = item.Icon;

        if (isNPCShopItem)
        {
            quantityContainer.gameObject.SetActive(false);
        }
    }

    public void ShowSlotInformation(bool value)
    {
        itemIcon.gameObject.SetActive(value);
        quantityContainer.gameObject.SetActive(value);
        equippedCharacterIcon.gameObject.SetActive(value);
    }
}