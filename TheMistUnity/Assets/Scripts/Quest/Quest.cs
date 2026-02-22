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
    public Sprite QuestGiverIcon;
    public string QuestGiverName;

    public bool IsMainQuest;
    
    [TextArea] public string Description;
    
    public QuestTask[] Tasks;

    [Header("Reward")]
    public int GoldReward;
    public float ExpReward;
    public QuestItemReward[] ItemReward;

    [HideInInspector] public bool QuestCompleted;
    [HideInInspector] public bool QuestAccepted;
    [HideInInspector] public bool QuestClaimed;
    
    public void AddProgress(int amount = 0)
    {
        foreach (QuestTask task in Tasks)
        {
            if (task.IsCompleted) continue;
            
            task.AddProgress(amount);
            break;
        }

        if (Tasks[^1].IsCompleted)
        {
            CompleteQuest();
        }

        UIManager.Instance.LoadQuestsUI();
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
        
        ResetTasks();
    }

    private void ResetTasks()
    {
        foreach (QuestTask task in Tasks)
        {
            task.ResetTask();
        }
    }
    
}

[Serializable]
public class QuestItemReward
{
    public InventoryItem Item;
    public int Quantity;
}
