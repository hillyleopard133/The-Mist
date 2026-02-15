using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAttack : FSMAction
{
    [Header("Config")]
    [SerializeField] public float damage;
    [SerializeField] private float timeBtwAttacks;  //time between attacks

    private EnemyBrain enemyBrain;
    private float timer;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
    }
    
    public override void Act()
    {
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        if (enemyBrain.Player == null)
        {
            return;
        }
        
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            enemyBrain.animations.SetMoveBoolTransition(false);
            //IDamageable player = enemyBrain.Player.GetComponent<IDamageable>();
            //player.TakeDamage(damage);
            timer = timeBtwAttacks;
        }
    }
    
}
