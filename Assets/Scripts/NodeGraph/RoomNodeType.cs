using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Temple/Room Node Type")]
public class RoomNodeType : ScriptableObject
{
    public string roomNodeTypeName;

    public bool displayInNodeGraphEditor = true;
    public bool isCorridor;
    public bool isCorridorNS;
    public bool isCorridorEW;
    public bool isEntrance;
    public bool isPuzzleRoom;
    public bool isBossRoom;
    public bool isLeverRoom;
    public bool isNone;
}
