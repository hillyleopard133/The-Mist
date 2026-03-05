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
    public int CurrentHealth;
    public AttackMove[] attackMoves;

    public DamageType[] weaknesses;
    public DamageType[] resistances;
    
    public EnemyDetails CopyEnemy()
    {
        EnemyDetails instance = Instantiate(this);
        return instance;
    }

    public float GetHealthBarPercentage()
    {
        return (float) CurrentHealth / MaxHealth;
    }
}
