using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private List<AttackMove> attackMoves;
    [SerializeField] private List<AttackEffect> inflictedEffects = new List<AttackEffect>();

    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }
    
    public bool HasEffect(AttackEffectType effectType)
    {
        foreach (AttackEffect effect in inflictedEffects)
        {
            if (effect == null) continue;
            if (effect.GetEffectType() == effectType)
            {
                return true;
            }
        }
        return false;
    }
    
    public void AddAttackMove(AttackMove attack)
    {
        attackMoves.Add(attack);
    }

    public AttackMove Attack()
    {
        if (HasEffect(AttackEffectType.Stun))
        {
            return null;
        }
        
        int randomAttack = Random.Range(0, attackMoves.Count);
        return attackMoves[randomAttack];
    }

    public float GetHealthPercentage()
    {
        return (Mathf.Round((enemyHealth.CurrentHealth / enemyHealth.health) * 10f) / 10f) * 100f;
    }

    public void ClearAllEffects()
    {
        inflictedEffects.Clear();
    }
    
    public void Heal(float amount)
    {
        enemyHealth.Heal(amount);
    }

    public List<AttackEffect> GetInflictedEffects()
    {
        return inflictedEffects;
    }

    public void RemoveEffect(AttackEffect effect)
    {
        inflictedEffects.Remove(effect);
    }

    public void AddEffect(AttackEffect effect)
    {
        inflictedEffects.Add(effect);
    }
    
    public List<AttackMove> GetAttacks()
    {
        return attackMoves;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

}
