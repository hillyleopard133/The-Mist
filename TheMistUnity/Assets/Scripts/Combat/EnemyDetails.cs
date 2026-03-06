using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ScriptableObjects/Combat/Enemy", fileName = "Enemy")]
public class EnemyDetails : ScriptableObject
{
    public string EnemyName;
    public Sprite EnemySprite;
    public GameObject enemyPrefab;
    public int CoinsReward;
    public int ExpReward;

    public int MaxHealth;
    [HideInInspector] public int CurrentHealth;
    public bool IsDead;
    
    public AttackMove[] attackMoves;
    public DamageType[] weaknesses;
    public DamageType[] resistances;

    [SerializeField] private float weaknessMultiplier = 1.3f;
    [SerializeField] private float resistanceMultiplier = 0.75f;
    
    public EnemyDetails CopyEnemy()
    {
        EnemyDetails instance = Instantiate(this);
        instance.CurrentHealth = MaxHealth;
        return instance;
    }

    public float GetHealthBarPercentage()
    {
        return (float) CurrentHealth / MaxHealth;
    }

    public bool TakeDamage(float damage, DamageType damageType)
    {
        foreach (DamageType weakness in weaknesses)
        {
            if(weakness == damageType) damage *= weaknessMultiplier;
        }
        foreach (DamageType resistance in resistances)
        {
            if (resistance == damageType) damage *= resistanceMultiplier;
        }
        
        CurrentHealth -= Mathf.RoundToInt(damage);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            IsDead = true;
        }
        return IsDead;
    }
    
}
