using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionDetectPlayer : FSMDecision
{

    [Header("Config")]
    [SerializeField] private float range;

    [SerializeField] private LayerMask playerMask;

    private EnemyBrain enemy;
    private ActionPatrol actionPatrol;
    private ActionWander actionWander;
    private ActionChase actionChase;

    private void Awake()
    {
        enemy = GetComponent<EnemyBrain>();
        actionPatrol = GetComponent<ActionPatrol>();
        actionWander = GetComponent<ActionWander>();
        actionChase = GetComponent<ActionChase>();
    }
    
    public override bool Decide()
    {
        return DetectPlayer();
    }

    private bool DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(enemy.transform.position, range, playerMask);
        if (playerCollider != null)
        {
            enemy.Player = playerCollider.transform;
            if (actionPatrol != null)
            {
               actionPatrol.StopMoving();
            }

            if (actionWander != null)
            {
                actionWander.StopMoving();
            }
            return true;
        }

        enemy.Player = null;
        actionChase.StopMoving();
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    
}
