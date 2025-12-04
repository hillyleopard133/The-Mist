using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class DialogueNode : ScriptableObject
{
    [HideInInspector] [SerializeField] public int speakerIndex;

    [SerializeField] private bool isPlayerSpeaking;
    [SerializeField] private bool isQuestOption;
    [SerializeField] private bool triggerInteraction;
    [TextArea] [SerializeField] private string text;
    [HideInInspector] [SerializeField] private List<string> children = new List<string>();
    [HideInInspector] [SerializeField] private Rect rect = new Rect(0, 0, 250, 100);

    [SerializeField] private List<DialogueRequirementOr> requirementsOr = new List<DialogueRequirementOr>();

    [SerializeField] private UnityEvent onTrigger;

    public void TriggerAction()
    {
        onTrigger.Invoke();
    }

    public bool CheckRequirements()
    {
        if (requirementsOr.Count == 0) return true;
        foreach (DialogueRequirementOr requirement in requirementsOr)
        {
            if (requirement.CheckRequirements())
            {
                return true;
            }
        }
        return false;
    }

    public string GetText()
    {
        return text;
    }

    public List<string> GetChildren()
    {
        return children;
    }

    public Rect GetRect()
    {
        return rect;
    }

    public bool IsQuestOption()
    {
        return isQuestOption;
    }

    public bool IsPlayerSpeaking()
    {
        return isPlayerSpeaking;
    }

    public bool IsTriggerInteraction()
    {
        return triggerInteraction;
    }


#if UNITY_EDITOR
    public void SetPosition(Vector2 newPosition)
    {
        Undo.RecordObject(this, "Moved node");
        rect.position = newPosition;
        EditorUtility.SetDirty(this);
    }

    public void SetText(string newText)
    {
        if (newText != text)
        {
            Undo.RecordObject(this, "Edit Dialogue");
            text = newText;
            EditorUtility.SetDirty(this);
        }
    }

    public void AddChild(string childID)
    {
        Undo.RecordObject(this, "Linked a child");
        children.Add(childID);
        EditorUtility.SetDirty(this);
    }

    public void RemoveChild(string childID)
    {
        Undo.RecordObject(this, "Removed a child");
        children.Remove(childID);
        EditorUtility.SetDirty(this);
    }

    public void SetPlayerSpeaking(bool value)
    {
        Undo.RecordObject(this, "Changed speaker");
        isPlayerSpeaking = value;
        EditorUtility.SetDirty(this);
    }
#endif
}


