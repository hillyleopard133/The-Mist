using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TextMeshProUGUI damageTMP;

    public void SetDamageText(float damage)
    {
        if (damage % 1 == 0)
        {
            damageTMP.text = damage.ToString("F0");
        }
        else
        {
            damageTMP.text = damage.ToString("F1");
        }
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
    
}
