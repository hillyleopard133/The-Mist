using System;
using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DialogueManager : Singleton<DialogueManager>
{
    public static event Action<InteractionType> OnExtraInteractionEvent;
    
    [Header("Config")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image npcIcon;
    [SerializeField] private TextMeshProUGUI npcNameTMP;
    [SerializeField] private TextMeshProUGUI npcDialogueTMP;

    [Header("Choices")] 
    [SerializeField] private CharacterSpeaker playerCharacter;
    [SerializeField] private Transform playerChoicesBox;
    [SerializeField] private GameObject choicePrefab;
    
    [Header("QuestManager")]
    [SerializeField] private DialogueQuestManager dialogueQuestManager;
    
    public NPCInteraction NPCSelected { get; set; }
    private List<string> npcs = new List<string>();
    private readonly string NPCS_SPOKEN_TO = "NPCS_SPOKEN_TO";

    private bool dialogueStarted;
    private PlayerActions actions;
    
    private Dialogue currentDialogue;
    private DialogueNode currentNode = null;
    private bool isChoosing;
    
    protected override void Awake()
    {
        base.Awake();
        if (actions == null)
        {
            actions = new PlayerActions();
        }
    }

    private void Start()
    {
        actions.Dialogue.Interact.performed += ctx => StartDialogue();
        actions.Dialogue.Continue.performed += ctx => Next();
        LoadNPCs();
    }

    public void EnableActions()
    {
        actions.Dialogue.Enable();
    }
    
    public void DisableActions()
    {
        actions.Dialogue.Disable();
    }
    

    public void ResetNPCs()
    {
        npcs.Clear();
    }

    private void LoadNPCs()
    {
        if (SaveGame.Exists(NPCS_SPOKEN_TO))
        {
            npcs = SaveGame.Load<List<string>>(NPCS_SPOKEN_TO);
        }
    }

    private void SaveNPCs()
    {
        SaveGame.Save(NPCS_SPOKEN_TO, npcs);
    }

    private void CheckNPC()
    {
        if (npcs.Contains(NPCSelected.name)) return;
        NPCSelected.ResetDialogueOptions();
        npcs.Add(NPCSelected.name);
        SaveNPCs();
    }

    public void ChangeStartDialogueOnEnter(bool value)
    {   
        NPCSelected.ChangeStartDialogueOnEnter(value);
    }

    public void ChangeDialogueIsLeavable(bool value)
    {
        NPCSelected.ChangeDialogueIsNotLeavable(value);
    }

    public DialogueQuestManager GetDialogueQuestManager()
    {
        return dialogueQuestManager;
    }
    
    public void SelectNPC(NPCInteraction npc)
    {
        NPCSelected = npc;
        if (NPCSelected != null)
        {
            CheckNPC();
            currentDialogue = NPCSelected.DialogueToShow;
        }
    }

    public CharacterSpeaker GetCurrentSpeaker()
    {
        return currentDialogue.GetCharacterSpeakers()[currentNode.speakerIndex];
    }

    public IEnumerable<DialogueNode> GetChoices()
    {
        return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
    }

    public void SelectChoice(DialogueNode chosenNode)
    {
        currentNode = chosenNode;
        TriggerAction();
        isChoosing = false;
        Next();
    }
    
    public void StartDialogue()
    {
        if (NPCSelected == null || dialogueStarted) return;
        
        currentNode = currentDialogue.GetRootNode();
        dialoguePanel.SetActive(true);
        TriggerAction();
        UpdateUI();
        dialogueStarted = true;
        if (NPCSelected.GetDialogueIsNotLeavable())
        {
            GameManager.Instance.DisablePlayerMovement();
        }
    }
    
    public void Next()
    {
        if (currentNode.IsTriggerInteraction())
        {
            InvokeInteraction();
            return;
        }
        if (!HasNext())
        {
            CloseDialoguePanel();
            return;
        }
        if (FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count() > 0)
        {
            isChoosing = true;
            UpdateUI();
            return;
        }
        
        DialogueNode[] children = FilterOnCondition(currentDialogue.GetNPCChildren(currentNode)).ToArray();
        int randomChild = Random.Range(0, children.Count());
        
        currentNode = children[randomChild];
        TriggerAction();
        UpdateUI();
    }
    
    public bool HasNext()
    {
        return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
    }
    
    private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> nodesToFilter)
    {
        foreach (DialogueNode node in nodesToFilter)
        {
            if (node.CheckRequirements())
            {
                yield return node;
            }
        }
    }

    private void UpdateUI()
    {
        if (isChoosing)
        {
            npcIcon.sprite = playerCharacter.Icon;
            npcNameTMP.text = playerCharacter.Name;
            BuildChoiceList();
            npcDialogueTMP.gameObject.SetActive(false);
            playerChoicesBox.gameObject.SetActive(true);
        }
        else
        {
            playerChoicesBox.gameObject.SetActive(false);
            npcDialogueTMP.gameObject.SetActive(true);
            CharacterSpeaker speaker = GetCurrentSpeaker();
            npcIcon.sprite = speaker.Icon;
            npcNameTMP.text = speaker.Name;
            npcDialogueTMP.text = currentNode.GetText();
        }
    }
    
    private void BuildChoiceList()
    {
        foreach (Transform choiceButton in playerChoicesBox)
        {
            Destroy(choiceButton.gameObject);
        }

        foreach (DialogueNode choiceNode in GetChoices())
        {
            GameObject choice = Instantiate(choicePrefab, playerChoicesBox);
            choice.GetComponentInChildren<TextMeshProUGUI>().text = choiceNode.GetText();
            Button choiceButton = choice.GetComponentInChildren<Button>();
        
            choiceButton.onClick.AddListener(() =>
            {
                SelectChoice(choiceNode);
            });

            if (choiceNode.IsQuestOption())
            {
                choice.GetComponent<Image>().color = new Color32(255, 150,0, 255);
            }
        }
    }
    
    private void TriggerAction()
    {
        currentNode.TriggerAction();
    }

    public void CloseDialoguePanel()
    {
        GameManager.Instance.EnablePlayerMovement();
        isChoosing = false;
        dialoguePanel.SetActive(false);
        dialogueStarted = false;
    }

    public void InvokeInteraction()
    {
        CloseDialoguePanel();
        if (NPCSelected.HasInteraction)
        {
            OnExtraInteractionEvent?.Invoke(NPCSelected.InteractionType);
        }
    }

    private void OnEnable()
    {
        if (actions != null)
        {
            actions.Enable();
        }
    }

    private void OnDisable()
    {
        if (actions != null)
        {
            actions.Disable();
        }
    }
}
