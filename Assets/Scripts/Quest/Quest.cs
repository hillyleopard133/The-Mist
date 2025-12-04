using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Quest : ScriptableObject
{
    [Header("Info")]
    public string Name;
    public string ID;
    public int QuestGoal;
    
    [Header("Description")]
    [TextArea] public string[] Descriptions;
    public bool hasProgress;

    [Header("Reward")]
    public int GoldReward;
    public float ExpReward;
    //public QuestItemReward ItemReward;

    [HideInInspector] public int CurrentStatus;
    [HideInInspector] public bool QuestCompleted;
    [HideInInspector] public bool QuestAccepted;
    [HideInInspector] public bool QuestClaimed;

    public string GetDescription()
    {
        if (hasProgress)
        {
            return Descriptions[0];
        }
        else
        {
            return Descriptions[CurrentStatus];
        }
    }
    public void AddProgress(int amount)
    {
        if (hasProgress)
        {
            CurrentStatus += amount;
            if (CurrentStatus >= QuestGoal)
            {
                CurrentStatus = QuestGoal;
                CompleteQuest();
            }
        }
        else
        {
            CurrentStatus++;
        }
    }

    public void CompleteQuest()
    {
        if (QuestCompleted)
        {
            return;
        }
        QuestCompleted = true;
    }

    public void ResetQuest()
    {
        QuestAccepted = false;
        QuestCompleted = false;
        QuestClaimed = false;
        CurrentStatus = 0;
    }
    
}

[Serializable]
public class QuestItemReward
{
    //public InventoryItem Item;
    public int Quantity;
}
