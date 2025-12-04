using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public enum RequirementType
{
    HasAcceptedQuest,
    HasCompletedQuest,
    HasItem,
    HasPassedDialogueTrigger
}

[System.Serializable]
public class DialogueRequirement
{
    [SerializeField] private RequirementType requirementType;
    [SerializeField] private bool not;
    [SerializeField] private Quest quest;
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] [Min(1)] private int amountOfItemRequired = 1;

    public bool MeetsRequirement()
    {
        switch (requirementType)
        {
            case RequirementType.HasAcceptedQuest:
                return CheckRequirement(quest.QuestAccepted);

            case RequirementType.HasCompletedQuest:
                return CheckRequirement(quest.QuestCompleted);
            
            case RequirementType.HasPassedDialogueTrigger:
                DialogueQuestManager dialogueQuestManager = DialogueManager.Instance.GetDialogueQuestManager();
                return CheckRequirement(dialogueQuestManager.dialogueTriggers.Contains(dialogueTrigger));
        }
        
        return false;
    }

    private bool CheckRequirement(bool reqLogic)
    {
        if (reqLogic)
        {
            if (not)
            {
                return false;
            }
            return true;
        }
        if (not)
        {
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class DialogueRequirementOr
{
    [SerializeField] private List<DialogueRequirement> requirements = new List<DialogueRequirement>();
    
    public bool CheckRequirements()
    {
        foreach (DialogueRequirement requirement in requirements)
        {
            if (!requirement.MeetsRequirement())
            {
                return false; 
            }
        }
        return true;
    }
}
