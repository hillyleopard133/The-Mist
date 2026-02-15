
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowAttackMoveDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SandwichMonsterAttackMove attack;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //SandwichFightManager.Instance.ShowAttackMoveDescription(attack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //SandwichFightManager.Instance.HideAttackMoveDescription();
    }
    
    public void SetAttack(SandwichMonsterAttackMove attackMove)
    {
        attack = attackMove;
    }
}