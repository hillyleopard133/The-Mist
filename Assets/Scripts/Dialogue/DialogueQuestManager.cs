using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/QuestManager")]
public class DialogueQuestManager : ScriptableObject
{
    [HideInInspector] public List<DialogueTrigger> dialogueTriggers = new List<DialogueTrigger>();
    private string DIALOGUE_TRIGGERS = "DIALOGUE_TRIGGERS";
    private int itemAmount;
    public void GiveQuest(Quest quest)
    {
        QuestManager.Instance.AcceptQuest(quest);
        quest.QuestAccepted = true;
    }

    public void CompleteQuest(Quest quest)
    {
        QuestManager.Instance.CompleteQuest(quest.ID);    
    }

    public void AddQuestProgress(Quest quest)
    {
        quest.AddProgress(0);
    }

    public void PassDialogueTrigger(DialogueTrigger trigger)
    {
        if (dialogueTriggers.Contains(trigger))
        {
            return;
        }
        dialogueTriggers.Add(trigger);
        SaveDialogueTriggers();
    }

    public void DisableInteractions()
    {
        DialogueManager.Instance.NPCSelected.SetShowInteractionBox(false);
        DialogueManager.Instance.NPCSelected.enabled = false;
    }
    
    public void ChangeStartDialogueOnEnter(bool value)
    {
        DialogueManager.Instance.ChangeStartDialogueOnEnter(value);
    }

    public void ChangeDialogueIsLeavable(bool value)
    {
        DialogueManager.Instance.ChangeDialogueIsLeavable(value);
    }

    public void SaveDialogueTriggers()
    {
        List<string> triggerNames = new List<string>();

        foreach (DialogueTrigger trigger in dialogueTriggers)
        {
            triggerNames.Add(trigger.name);
        }

        SaveGame.Save(DIALOGUE_TRIGGERS, triggerNames);
    }

    public void LoadDialogueTriggers()
    {
        if (SaveGame.Exists(DIALOGUE_TRIGGERS))
        {
            List<string> triggerNames = SaveGame.Load<List<string>>(DIALOGUE_TRIGGERS);
            dialogueTriggers.Clear();

            foreach (string triggerName in triggerNames)
            {
                DialogueTrigger trigger = Resources.Load<DialogueTrigger>(triggerName);
                if (trigger != null)
                {
                    dialogueTriggers.Add(trigger);
                }
            }
        }
    }

    public void ResetDialogueTriggers()
    {
        dialogueTriggers.Clear();
        SaveDialogueTriggers();
    }
    
}