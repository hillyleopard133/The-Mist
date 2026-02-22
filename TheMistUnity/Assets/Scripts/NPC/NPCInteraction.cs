using System;
using BayatGames.SaveGameFree;
using Unity.VisualScripting;
using UnityEngine;

public enum InteractionType
{
    Quest,
    SuppliesShop,
    EquipmentShop,
    Crafting
}

public class NPCInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public bool HasInteraction;
    public InteractionType InteractionType;
    
    [SerializeField] private Dialogue Dialogue;
    
    [Header("Other options")]
    [SerializeField] private bool startDialogueOnEnterInitial;
    private bool startDialogueOnEnter;
    [SerializeField] private bool dialogueIsNotLeavableInitial;
    private bool dialogueIsNotLeavable;
    
    [Header("Quests")]
    [SerializeField] private Quest[] quests;
    [SerializeField] public InventoryItem[] shop;
    
    [SerializeField] private GameObject interactionBox;
    private bool showInteractionBox = true;
    
    private string START_DIALOGUE_ON_ENTER;
    private string DIALOGUE_IS_NOT_LEAVABLE;    
    private string DIALOGUE_BOX_IS_SHOWN;
    
    public Dialogue DialogueToShow => Dialogue;

    private void Start()
    {
        START_DIALOGUE_ON_ENTER = "START_DIALOGUE_ON_ENTER" + gameObject.name;
        DIALOGUE_IS_NOT_LEAVABLE = "FIRST_START" + gameObject.name;
        DIALOGUE_BOX_IS_SHOWN = "DIALOGUE_BOX_IS_SHOWN" + gameObject.name;
        LoadOptions();
    }
    
    public bool GetStartDialogueOnEnter() => startDialogueOnEnter;
    public bool GetDialogueIsNotLeavable() => dialogueIsNotLeavable;

    public void ResetDialogueOptions()
    {
        startDialogueOnEnter = startDialogueOnEnterInitial;
        dialogueIsNotLeavable = dialogueIsNotLeavableInitial;
        showInteractionBox = true;
        SaveOptions();
    }

    private void LoadOptions()
    {
        if (SaveGame.Exists(START_DIALOGUE_ON_ENTER))
        {
            startDialogueOnEnter = SaveGame.Load<bool>(START_DIALOGUE_ON_ENTER);
        }

        if (SaveGame.Exists(DIALOGUE_IS_NOT_LEAVABLE))
        {
            dialogueIsNotLeavable = SaveGame.Load<bool>(DIALOGUE_IS_NOT_LEAVABLE);
        }

        if (SaveGame.Exists(DIALOGUE_BOX_IS_SHOWN))
        {
            showInteractionBox = SaveGame.Load<bool>(DIALOGUE_BOX_IS_SHOWN);
        }
    }

    private void SaveOptions()
    {
        SaveGame.Save(START_DIALOGUE_ON_ENTER, startDialogueOnEnter);
        SaveGame.Save(DIALOGUE_IS_NOT_LEAVABLE, dialogueIsNotLeavable);
        SaveGame.Save(DIALOGUE_BOX_IS_SHOWN, showInteractionBox);
    }

    public void ChangeStartDialogueOnEnter(bool value)
    {
        startDialogueOnEnter = value;
        SaveOptions();
    }

    public void ChangeDialogueIsNotLeavable(bool value)
    {
        dialogueIsNotLeavable = value;
        SaveOptions();
    }
    
    public void SetShowInteractionBox(bool show)
    {
        showInteractionBox = show;
        interactionBox.SetActive(show);
        SaveOptions();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.SelectNPC(this);
            QuestManager.Instance.Quests = quests;
            QuestManager.Instance.LoadQuestsIntoNPCPanel();
            
            if (startDialogueOnEnter)
            {
                DialogueManager.Instance.StartDialogue();
            }

            if (showInteractionBox)
            {
                interactionBox.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.SelectNPC(null);
            DialogueManager.Instance.CloseDialoguePanel();
            interactionBox.SetActive(false);
        }
    }
    
}