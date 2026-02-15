using System;

[Serializable]
public class FSMTransition
{
    public FSMDecision Decision; //Player is in range of attack?
    public string TrueState;    //If true change current state to attack state
    public string FalseState;   //if false current state goes to patrol
    
    
}