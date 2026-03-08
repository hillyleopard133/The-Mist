using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyAction
{
    Heal,
    Attack,
    ChargeAttack,
    Defend,
    Rest
}

public class EnemyCombatBrain : MonoBehaviour
{
    [HideInInspector] public EnemyDetails enemyDetails;

    private int charge;
    private EnemyAction lastAction = EnemyAction.Rest;
    private int targetPartyMemberIndex;
    private AttackMove selectedAttack;
    
    private CombatManager combatManager;

    private void Start()
    {
        combatManager = CombatManager.Instance;
    }
    
    public IEnumerator TakeTurn()
    {
        EnemyAction currentAction = EvaluateActions();

        switch (currentAction)
        {
            case EnemyAction.Attack:
                combatManager.AttackPartyMember(targetPartyMemberIndex, selectedAttack, enemyDetails);
                break;
            case EnemyAction.Defend:
                break;
            case EnemyAction.Heal:
                break;
            case EnemyAction.ChargeAttack:
                charge--;
                break;
            case EnemyAction.Rest:
                break;
        }
        
        yield return null;
        
        
        lastAction = currentAction;
    }

    private EnemyAction EvaluateActions()
    {
        EnemyAction currentAction = EnemyAction.Attack;
        
        AttackMove[] attackMoves = enemyDetails.attackMoves;
        selectedAttack = attackMoves[0];

        targetPartyMemberIndex = combatManager.GetHighestHPPartyMember();
        
        return currentAction;
    }
    

}