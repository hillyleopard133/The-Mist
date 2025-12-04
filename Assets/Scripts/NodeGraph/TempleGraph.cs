using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "TempleGraph", menuName = "Scriptable Objects/Temple/Room Node Graph")]
public class TempleGraph : Graph
{
    public TempleRoomNode GetRoomNode(RoomNodeType roomNodeType)
    {
        foreach (TempleRoomNode roomNode in nodes)
        {
            if (roomNode.roomNodeType == roomNodeType)
            {
                return roomNode;
            }
        }
        return null;
    }
    
    public TempleRoomNode GetRoomNode(string id)
    {
        nodesDictionary.TryGetValue(id, out GraphNode node);
        return (TempleRoomNode)node;
    }
    
    protected override GraphNode MakeNode(GraphNode parent)
    {
        TempleRoomNode newNode = CreateInstance<TempleRoomNode>();
        newNode.name = Guid.NewGuid().ToString();
        if (parent != null)
        {
            parent.AddChild(newNode.name);
            newNode.SetPosition(parent.GetRect().position + newNodeOffset);
        }

        return newNode;
    }
    
}
