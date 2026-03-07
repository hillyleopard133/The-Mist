using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum CombatTextType
{
    Damage,
    ManaRecovery,
    HealthRecovery
}

public class CombatText : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Color damageColor;
    [SerializeField] private Color manaRecoveryColor;
    [SerializeField] private Color healthRecoveryColor;

    public void SetDamageText(int amount, CombatTextType type)
    {
        string text = "";

        switch (type)
        {
            case CombatTextType.Damage:
                amountText.color = damageColor;
                break;
            case CombatTextType.ManaRecovery:
                amountText.color = manaRecoveryColor;
                text += "+";
                break;
            case CombatTextType.HealthRecovery:
                amountText.color = healthRecoveryColor;
                text += "+";
                break;
        }
        
        text += amount.ToString();
        
        amountText.text = text;
    }

    public void DisableText()
    {
        gameObject.SetActive(false);
    }
    
}
