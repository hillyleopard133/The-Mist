using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/Quest", fileName = "Quest")]
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
    public int CoinReward;
    public int ExpReward;
    public QuestItemReward[] ItemRewards;

    [HideInInspector] public bool QuestCompleted;
    [HideInInspector] public bool QuestAccepted;
    
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
        GiveQuestRewards();
        
        QuestManager.Instance.SaveQuestData();
        UIManager.Instance.LoadQuestsUI();
    }

    private void GiveQuestRewards()
    {
        SkillsManager.Instance.AddExp(ExpReward);
        CoinManager.Instance.AddCoins(CoinReward);

        foreach (QuestItemReward itemReward in ItemRewards)
        {
            Inventory.Instance.AddItem(itemReward.Item, itemReward.Quantity);
        }
    }

    public void ResetQuest()
    {
        QuestAccepted = false;
        QuestCompleted = false;
        
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
