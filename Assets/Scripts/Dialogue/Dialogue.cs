using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private List<CharacterSpeaker> characterSpeakers = new List<CharacterSpeaker>();
    [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
    [HideInInspector] [SerializeField] private Vector2 newNodeOffset = new Vector2(350, 0);

    [HideInInspector] [SerializeField] Dictionary<string, DialogueNode> nodesDictionary = new Dictionary<string, DialogueNode>();

    public IEnumerable<string> GetCharacterNames()
    {
        foreach (CharacterSpeaker characterSpeaker in characterSpeakers)
        {
            yield return characterSpeaker.Name;
        }
    }

    public List<CharacterSpeaker> GetCharacterSpeakers()
    {
        return characterSpeakers;
    }

    private void OnValidate()
    {
        nodesDictionary.Clear();
        foreach (DialogueNode node in GetAllNodes())
        {
            if (node != null)
            {
                nodesDictionary[node.name] = node;
            }
        }
    }

    public IEnumerable<DialogueNode> GetAllNodes()
    {
        return nodes;
    }

    public DialogueNode GetRootNode()
    {
        return nodes[0];
    }

    public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
    {
        foreach (string id in parentNode.GetChildren())
        {
            // yield return will create the list for you if return type is IEnumerable,
            // makes it much neater than creating a new list, adding the nodes to it and then returning the list at the end.
            if (nodesDictionary.ContainsKey(id))
            {
                yield return nodesDictionary[id];
            }
        }
    }

    public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parentNode)
    {
        foreach (DialogueNode node in GetAllChildren(parentNode))
        {
            if (node.IsPlayerSpeaking())
            {
                yield return node;
            }
        }
    }

    public IEnumerable<DialogueNode> GetNPCChildren(DialogueNode parentNode)
    {
        foreach (DialogueNode node in GetAllChildren(parentNode))
        {
            if (!node.IsPlayerSpeaking())
            {
                yield return node;
            }
        }
    }

    public void OnEnable()
    {
        OnValidate();
    }


#if UNITY_EDITOR
    public void CreateNode(DialogueNode parent)
    {
        DialogueNode newNode = MakeNode(parent);
        Undo.RegisterCreatedObjectUndo(newNode, "Created New Dialogue Node");
        Undo.RecordObject(this, "Add a new dialogue node");
        AddNode(newNode);
    }

    private void AddNode(DialogueNode newNode)
    {
        nodes.Add(newNode);
        OnValidate();
    }

    private DialogueNode MakeNode(DialogueNode parent)
    {
        DialogueNode newNode = CreateInstance<DialogueNode>();
        newNode.name = Guid.NewGuid().ToString();
        if (parent != null)
        {
            parent.AddChild(newNode.name);
            newNode.SetPosition(parent.GetRect().position + newNodeOffset);
        }

        return newNode;
    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        Undo.RecordObject(this, "Delete a node");
        nodes.Remove(nodeToDelete);
        OnValidate();
        foreach (DialogueNode node in GetAllNodes())
        {
            node.RemoveChild(nodeToDelete.name);
        }

        Undo.DestroyObjectImmediate(nodeToDelete);

    }
#endif

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (nodes.Count == 0)
        {
            DialogueNode newNode = MakeNode(null);
            AddNode(newNode);
        }

        if (AssetDatabase.GetAssetPath(this) != "")
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                if (AssetDatabase.GetAssetPath(node) == "")
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
        }
#endif
    }

    public void OnAfterDeserialize()
    {
    }
}


