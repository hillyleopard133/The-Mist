using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphNode : ScriptableObject
{
    [HideInInspector] [SerializeField] protected List<string> children = new List<string>();
    [HideInInspector] [SerializeField] private Rect rect = new Rect(0, 0, 250, 100);
    
    public bool IsNodeStyle2()
    {
        return false;
    }
    
    public virtual void DrawNode(){}
    
    public List<string> GetChildren()
    {
        return children;
    }

    public Rect GetRect()
    {
        return rect;
    }
    
#if UNITY_EDITOR
    public void SetPosition(Vector2 newPosition)
    {
        Undo.RecordObject(this, "Moved node");
        rect.position = newPosition;
        EditorUtility.SetDirty(this);
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
    
#endif
}
