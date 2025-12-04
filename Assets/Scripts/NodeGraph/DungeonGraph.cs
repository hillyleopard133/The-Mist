using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGraph", menuName = "Scriptable Objects/Temple/Dungeon Graph")]
public class DungeonGraph : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeList roomNodeTypeList;
    [HideInInspector] public List<RoomNode> roomNodeList = new List<RoomNode>();
    [HideInInspector] public Dictionary<string, RoomNode> roomNodeDictionary = new Dictionary<string, RoomNode>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        foreach (RoomNode node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    public RoomNode GetRoomNode(RoomNodeType roomNodeType)
    {
        foreach (RoomNode roomNode in roomNodeList)
        {
            if (roomNode.roomNodeType == roomNodeType)
            {
                return roomNode;
            }
        }
        return null;
    }

    public RoomNode GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNode roomNode))
        {
            return roomNode;
        }
        return null;
    }

    public IEnumerable<RoomNode> GetChildRoomNodes(RoomNode parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNode(childNodeID);
        }
    }

#if UNITY_EDITOR

    [HideInInspector] public RoomNode roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNode roomNode, Vector2 position)
    {
        roomNodeToDrawLineFrom = roomNode;
        linePosition = position;
    }

#endif
}
