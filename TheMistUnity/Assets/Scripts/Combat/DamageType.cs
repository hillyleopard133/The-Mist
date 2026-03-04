using UnityEngine;

public enum DamageTypes
{
    None,
    Fire,
    Ice,
    Storm,
    
    Blunt,
    Sharp,
    Magic
}

[CreateAssetMenu(menuName = "ScriptableObjects/Combat/DamageType", fileName = "DamageType")]
public class DamageType : ScriptableObject
{
    public DamageTypes damageType;
    public Sprite icon;
}

