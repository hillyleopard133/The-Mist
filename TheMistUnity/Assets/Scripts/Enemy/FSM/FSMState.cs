using System;

[Serializable]
public class FSMState 
{
    public string ID;
    public FSMAction[] Actions; 
    public FSMTransition[] Transitions;

    public void UpdateState(EnemyBrainRPG enemyBrainRpg)
    {
        ExecuteActions();
        ExecuteTransitions(enemyBrainRpg);
    }

    private void ExecuteTransitions(EnemyBrainRPG enemyBrainRpg)
    {
        if (Transitions == null || Transitions.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < Transitions.Length; i++)
        {
            bool value = Transitions[i].Decision.Decide();
            if (value)
            {
                enemyBrainRpg.ChangeState(Transitions[i].TrueState);
            }
            else
            {
                enemyBrainRpg.ChangeState(Transitions[i].FalseState);
            }
        }
    }
    
    private void ExecuteActions()
    {
        for (int i = 0; i < Actions.Length; i++)
        {
            Actions[i].Act();
        }    
    }
    
}