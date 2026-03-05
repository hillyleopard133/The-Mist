using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemAmount;

    private ItemConsumable item;
    private UIManager uIManager;

    private void Start()
    {
        uIManager = UIManager.Instance;
    }

    public void FillDetails(ItemConsumable consumable, int amount)
    {
        itemIcon.sprite = consumable.Icon;
        itemName.text = consumable.Name;
        itemAmount.text = amount.ToString();
        item = consumable;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        uIManager.ShowCombatItemInfo(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uIManager.HideCombatActionInfo();
    }
}
