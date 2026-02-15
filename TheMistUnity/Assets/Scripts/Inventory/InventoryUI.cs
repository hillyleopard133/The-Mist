using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : Singleton<InventoryUI>
{
    [Header("Config")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private Transform container;
    
    [Header("Description Panel")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameTMP;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;

    public InventorySlot CurrentSlot { get; set; }
    
    private List<InventorySlot> slotList = new List<InventorySlot>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitialiseInventory();
    }

    public bool IsInventoryOpen()
    {
        return inventoryPanel.activeSelf;
    }

    public void DrawInventory(InventoryItem[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            DrawItem(items[i], i);
        }

        for (int i = items.Length; i < slotList.Count; i++)
        {
            DrawItem(null, i);
        }
    }
    
    private void InitialiseInventory()
    {
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, container);
            slot.Index = i;
            slotList.Add(slot);
        }
    }

    private void MaintainSelectedSlot()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(CurrentSlot.gameObject);
        }
    }

    public void UseItem()
    {
        if (CurrentSlot == null)
        {
            return;
        }
        
        Inventory.Instance.UseItem(CurrentSlot.Index);
        MaintainSelectedSlot();
    }

    public void RemoveItem()
    {
        if (CurrentSlot == null)
        {
            return;
        }

        Inventory.Instance.RemoveItem(CurrentSlot.Index);
    }

    public void EquipItem()
    {
        if (CurrentSlot == null)
        {
            return;
        }
        
        Inventory.Instance.EquipItem(CurrentSlot.Index);
        MaintainSelectedSlot();
    }

    public void DrawItem(InventoryItem item, int index)
    {
        InventorySlot slot = slotList[index];
        if (item == null)
        {
            HideDescriptionPanel();
            slot.ShowSlotInformation(false);
            return;
        }
        slot.ShowSlotInformation(true);
        slot.UpdateSlot(item);
    }

    private void HideDescriptionPanel()
    {
        descriptionPanel.SetActive(false);
    }

    public void ShowItemDescription(int index)
    {
        InventoryItem[] items = Inventory.Instance.InventoryItems;
        if (Inventory.Instance.IsInventoryFiltered())
        {
            items = Inventory.Instance.FilteredItems;
        }

        if (index >= items.Length)
        {
            HideDescriptionPanel();
            return;
        }
        
        if (items[index] == null)
        {
            HideDescriptionPanel();
            return;
        }
        
        descriptionPanel.SetActive(true);
        itemIcon.sprite = items[index].Icon;
        itemNameTMP.text = items[index].Name;
        itemDescriptionTMP.text = items[index].Description;
    }

    public void OpenCloseInventory()
    {
        //.active self returns bool of whether it is active or not
        bool isActive = inventoryPanel.activeSelf;
        UIManager.Instance.CloseAllPanels();
        inventoryPanel.SetActive(!isActive);
        if (isActive == false)
        {
            descriptionPanel.SetActive(false);
            CurrentSlot = null;
            GameManager.Instance.DisablePlayerMovement();
        }
        else
        {
            GameManager.Instance.EnablePlayerMovement();
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        CurrentSlot = null;
        GameManager.Instance.EnablePlayerMovement();
        Inventory.Instance.RemoveFilter();
    }
    
    private void SlotSelectedCallback(int slotIndex)
    {
        CurrentSlot = slotList[slotIndex];
        ShowItemDescription(slotIndex);
    }
    
    private void OnEnable()
    {
        InventorySlot.OnSlotSelectedEvent += SlotSelectedCallback;
    }

    private void OnDisable()
    {
        InventorySlot.OnSlotSelectedEvent -= SlotSelectedCallback;
    }
}