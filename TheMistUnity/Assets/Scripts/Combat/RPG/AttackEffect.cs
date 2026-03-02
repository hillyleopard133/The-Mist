using UnityEngine;

public enum AttackEffectType
{
    Poison,
    Stun
}

[CreateAssetMenu (menuName = "Sandwich/AttackEffect")]
public class AttackEffect : ScriptableObject
{
    [SerializeField] private AttackEffectType effect;
    [SerializeField] private Sprite icon;
    [SerializeField] private int effectDuration;

    public void ReduceEffectDuration()
    {
        effectDuration--;
    }

    public int GetEffectDuration()
    {
        return effectDuration;
    }
    
    public AttackEffect CopyEffect()
    {
        AttackEffect instance = Instantiate(this);
        return instance;
    }

    public AttackEffectType GetEffectType()
    {
        return effect;
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}