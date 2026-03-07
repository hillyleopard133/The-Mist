using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AttackType
{
    Basic,
    Skill
}

public enum AttackMoveType
{
    SingleTarget,
    AOE,
    MultiHit
}

[CreateAssetMenu (menuName = "ScriptableObjects/Combat/AttackMove", fileName = "AttackMove")]
public class AttackMove: ScriptableObject
{
    public string MoveName;
    [TextArea] public string Description;
    public AttackType Type;
    public AttackMoveType MoveType;
    public DamageType DamageType;
    public float DamageMultiplier;
    public int MPCost;

    public bool IsHitAll;
    
    public bool HasEffect;
    public AttackEffect Effect;
    public float EffectChance;
    
    public void InflictEffect(EnemyCombat enemyCombat)
    {
        int random = Random.Range(0, 100);
        if (random <= EffectChance)
        {
            if(HasEffect) enemyCombat.AddEffect(Effect.CopyEffect());
        }
    }

}