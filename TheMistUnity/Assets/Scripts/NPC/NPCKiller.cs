using System;
using UnityEngine;

public class NPCKiller : MonoBehaviour
{
    [SerializeField] private DialogueTrigger killTrigger;
    [SerializeField] private DialogueTrigger reviveTrigger;

    private void Start()
    {
        if (!DialogueManager.Instance.GetDialogueQuestManager().dialogueTriggers.Contains(reviveTrigger))
        {
            GetComponent<NPCFollowPlayer>().KillNPC();
        }
    }
}