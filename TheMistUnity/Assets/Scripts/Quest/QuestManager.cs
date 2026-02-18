using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [HideInInspector] public Quest[] Quests { get; set; }
    public List<Quest> acceptedQuests;
    
    [SerializeField] public Quest[] AllQuests;
    
    [Header("NPC Quest Panel")]
    [SerializeField] private QuestCardNPC questCardNPCPrefab;
    [SerializeField] private Transform npcPanelContainer;
    
    private readonly string QUEST_DATA = "QUEST_DATA";

    private void Start()
    {
        acceptedQuests = new List<Quest>();
    }

    public void AcceptQuest(Quest quest)
    {
        acceptedQuests.Add(quest);
        AudioManager.Instance.PlayAcceptQuestSound();
        if (!quest.IsMainQuest)
        {
            UIManager.Instance.UpdateSideQuestList();
        }
        quest.QuestAccepted = true;
        SaveQuestData();
    }

    public void AddProgress(string questID, int amount = 0)
    {
        Quest questToUpdate = QuestExists(questID);
        if (questToUpdate == null)
        {
            return;
        }

        if (questToUpdate.QuestAccepted)
        {
            questToUpdate.AddProgress(amount);
        }
        
        SaveQuestData();
    }

    public void CompleteQuest(string questID)
    {
        Quest questToUpdate = QuestExists(questID);
        if (questToUpdate == null)
        {
            return;
        }

        if (questToUpdate.QuestAccepted)
        {
            questToUpdate.CompleteQuest();
        }
    }

    private Quest QuestExists(string questID)
    {
        foreach (Quest quest in acceptedQuests)
        {
            if (quest.ID == questID)
            {
                return quest;
            }
        }
        
        return null;
    }

    public void LoadQuestsIntoNPCPanel()
    {
        foreach (Transform child in npcPanelContainer)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < Quests.Length; i++)
        {
            if (!Quests[i].QuestAccepted)
            {
                QuestCard npcCard = Instantiate(questCardNPCPrefab, npcPanelContainer);
                npcCard.ConfigQuestUI(Quests[i]);
            }
        }
    }

    public void LoadQuestData()
    {
        if (SaveGame.Exists(QUEST_DATA))
        {
            acceptedQuests.Clear();
            QuestData questData = SaveGame.Load<QuestData>(QUEST_DATA);
            for (int i = 0; i < AllQuests.Length; i++)
            {
                AllQuests[i].QuestAccepted = questData.QuestAccepted[i];
                AllQuests[i].QuestCompleted = questData.QuestCompleted[i];
                AllQuests[i].QuestClaimed = questData.QuestClaimed[i];
                
                if (AllQuests[i].QuestAccepted && !AllQuests[i].QuestClaimed)
                {
                    acceptedQuests.Add(AllQuests[i]);
                }
            }
            UIManager.Instance.LoadQuestsUI();
        }
    }

    public void SaveQuestData()
    {
        QuestData questData = new QuestData();
        questData.QuestAccepted = new bool[AllQuests.Length];
        questData.QuestCompleted = new bool[AllQuests.Length];
        questData.CurrentStatus = new int[AllQuests.Length];
        questData.QuestClaimed = new bool[AllQuests.Length];

        for (int i = 0; i < AllQuests.Length; i++)
        {
            questData.QuestAccepted[i] = AllQuests[i].QuestAccepted;
            questData.QuestCompleted[i] = AllQuests[i].QuestCompleted;
            questData.QuestClaimed[i] = AllQuests[i].QuestClaimed;
        }
        SaveGame.Save(QUEST_DATA, questData);
    }

    public void ResetQuests()
    {
        acceptedQuests.Clear();
        foreach (Quest quest in AllQuests)
        {
            quest.ResetQuest();
        }
        UIManager.Instance.LoadQuestsUI();
    }
    

}