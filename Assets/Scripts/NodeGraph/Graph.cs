using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graph : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] protected List<GraphNode> nodes = new List<GraphNode>();
    [HideInInspector] [SerializeField] protected Dictionary<string, GraphNode> nodesDictionary = new Dictionary<string, GraphNode>();
    [HideInInspector] [SerializeField] protected Vector2 newNodeOffset = new Vector2(350, 0);
    
    public IEnumerable<GraphNode> GetAllNodes()
    {
        return nodes;
    }
    
    public GraphNode GetRootNode()
    {
        return nodes[0];
    }
    
    public IEnumerable<GraphNode> GetAllChildren(GraphNode parentNode)
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

    public GraphNode GetNode(string id)
    {
        nodesDictionary.TryGetValue(id, out GraphNode node);
        return node;
    }
    
    private void OnValidate()
    {
        nodesDictionary.Clear();
        foreach (GraphNode node in GetAllNodes())
        {
            if (node != null)
            {
                nodesDictionary[node.name] = node;
            }
        }
    }
    
    public void OnEnable()
    {
        OnValidate();
    }

#if UNITY_EDITOR
    public void CreateNode(GraphNode parent)
    {
        GraphNode newNode = MakeNode(parent);
        Undo.RegisterCreatedObjectUndo(newNode, "Created New Node");
        Undo.RecordObject(this, "Add a new node");
        AddNode(newNode);
    }

    protected void AddNode(GraphNode newNode)
    {
        nodes.Add(newNode);
        OnValidate();
    }

    protected virtual GraphNode MakeNode(GraphNode parent)
    {
        GraphNode newNode = CreateInstance<GraphNode>();
        newNode.name = Guid.NewGuid().ToString();
        if (parent != null)
        {
            parent.AddChild(newNode.name);
            newNode.SetPosition(parent.GetRect().position + newNodeOffset);
        }

        return newNode;
    }

    public void DeleteNode(GraphNode nodeToDelete)
    {
        Undo.RecordObject(this, "Delete a node");
        nodes.Remove(nodeToDelete);
        OnValidate();
        foreach (GraphNode node in GetAllNodes())
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
            GraphNode newNode = MakeNode(null);
            AddNode(newNode);
        }

        if (AssetDatabase.GetAssetPath(this) != "")
        {
            foreach (GraphNode node in GetAllNodes())
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
