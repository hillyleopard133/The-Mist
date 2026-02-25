using System;
using UnityEngine;
using Random = UnityEngine.Random;


public enum DamageTypes
{
    Fire,
    Ice,
    Wind,
    Blunt,
    Sharp,
    Magic
}


[CreateAssetMenu (menuName = "ScriptableObjects/Combat/AttackMove", fileName = "AttackMove")]
public class AttackMove: ScriptableObject
{
    [SerializeField] private float damage;
    [SerializeField] private bool hasEffect;
    [SerializeField] private AttackEffect effect;
    [SerializeField] private float effectChance;
    [SerializeField] private float hitChance;
    [SerializeField] private string moveName;

    public string GetMoveName()
    {
        return moveName;
    }

    public float GetDamage()
    {
        return damage;
    }

    public void InflictEffect(EnemyCombat enemyCombat)
    {
        int random = Random.Range(0, 100);
        if (random <= effectChance)
        {
            if(hasEffect) enemyCombat.AddEffect(effect.CopyEffect());
        }
    }

    public float GetHitChance()
    {
        return hitChance;
    }

    public float GetEffectChance()
    {
        return effectChance;
    }

    public bool HasEffect()
    {
        return hasEffect;
    }

    public AttackEffect GetEffect()
    {
        return effect;
    }
}