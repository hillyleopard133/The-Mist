using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class QuestTask : ScriptableObject
{
    public string Details;
    public bool HasNumber;
    public int CompletionNumber;
    
    [HideInInspector] public int CurrentNumber;
    [HideInInspector] public bool IsCompleted;

    public void ResetTask()
    {
        CurrentNumber = 0;
        IsCompleted = false;
    }

    public void AddProgress(int amount)
    {
        if (HasNumber)
        {
            CurrentNumber += amount;
            if (CurrentNumber >= CompletionNumber)
            {
                IsCompleted = true;
            }
        }
        else
        {
            IsCompleted = true;
        }
    }

    public string GetDetails()
    {
        if(!HasNumber) return Details;
        
        return Details + ": " + CurrentNumber + "/" + CompletionNumber;
    }
}
