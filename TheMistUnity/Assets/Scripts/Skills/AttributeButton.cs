using System;
using UnityEngine;

public class AttributeButton : MonoBehaviour
{
    public static event Action<AttributeType, int, bool> OnAttributeSelectedEvent;
    
    [Header("Config")]
    [SerializeField] private AttributeType attribute;
    [SerializeField] private int partyMember;
    [SerializeField] private bool isIncrease;

    public void SelectAttribute()
    {
        OnAttributeSelectedEvent?.Invoke(attribute, partyMember, isIncrease);
    }

    
}
