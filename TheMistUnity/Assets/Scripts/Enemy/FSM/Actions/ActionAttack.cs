using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAttack : FSMAction
{
    private EnemyBrain enemyBrain;

    private bool inCombat;

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
        if(inCombat) return;
        if (enemyBrain.Player == null) return; 
        
        inCombat = true;
        enemyBrain.animations.SetMoveBoolTransition(false);
        CombatManager.Instance.enemyArea.StartCombat();
    }
    
}
