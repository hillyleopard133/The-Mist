using UnityEngine;
using UnityEngine.EventSystems;

public class AttackMoveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AttackMove attackMove;
    private UIManager uIManager;

    private void Start()
    {
        uIManager = UIManager.Instance;
    }

    public void Instantiate(AttackMove attackMove)
    {
        this.attackMove = attackMove;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        uIManager.ShowCombatMoveInfo(attackMove);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uIManager.HideCombatActionInfo();
    }
}