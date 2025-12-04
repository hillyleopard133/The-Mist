using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleRoomNode : GraphNode
{
    public RoomNodeType roomNodeType;
    [HideInInspector] public RoomNodeTypeList roomNodeTypeList;
    public TempleGraph templeGraph;
    public TempleRoomNode parent;
    
#if  UNITY_EDITOR

    private bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;

        foreach (TempleRoomNode roomNode in templeGraph.GetAllNodes())
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parent != null)
            {
                isConnectedBossNodeAlready = true;
            }
        }

        if (templeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready) return false;
        if (templeGraph.GetRoomNode(childID).roomNodeType.isNone) return false;
        if(children.Contains(childID)) return false;
        if (name == childID) return false;
        if(parent.name == childID) return false;
        if(templeGraph.GetRoomNode(childID).parent != null) return false;
        if(templeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor) return false;
        if(!templeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor) return false;
        if(templeGraph.GetRoomNode(childID).roomNodeType.isCorridor && children.Count >= GameResources.maxChildCorridors) return false;
        if(templeGraph.GetRoomNode(childID).roomNodeType.isEntrance) return false;
        if(!templeGraph.GetRoomNode(childID).roomNodeType.isCorridor && children.Count > 0) return false;
        
        return true;
    }
    
    public void Initialise(TempleGraph templeGraph, RoomNodeType roomNodeType)
    {
        this.templeGraph = templeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        
        return roomArray;
    }
    
#endif
}
