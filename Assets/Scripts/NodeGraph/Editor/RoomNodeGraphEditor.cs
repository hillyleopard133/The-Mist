using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.XR;

public class RoomNodeGraphEditor : EditorWindow
{
    private static DungeonGraph _currentDungeonGraph;
    private RoomNode currentRoomNode = null;
    private RoomNodeTypeList roomNodeTypeList;

    //Graph and canvas space
    private Vector2 graphOffset;
    private Vector2 graphDrag;
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;
    
    //Node style
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    
    //Connecting line
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;
    
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Temple Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
    
    private void OnGUI()
    {
        if (_currentDungeonGraph != null)
        {
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);
            DrawDraggedLine();
            ProcessEvents(Event.current);
            DrawRoomNodeConnections();
            DrawRoomNodes();
        }
    }

    private void DrawBackgroundGrid(float gridSpacing, float gridOpacity, Color color)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSpacing) / gridSpacing);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSpacing) / gridSpacing);

        Handles.color = new Color(color.r, color.g, color.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;
        
        Vector3 gridOffset = new Vector3(graphOffset.x % gridSpacing, graphOffset.y % gridSpacing, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + gridOffset,
                new Vector3(gridSpacing * i, position.height + gridSpacing, 0) + gridOffset);
        }
        
        for (int i = 0; i < horizontalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0) + gridOffset,
                new Vector3(position.width + gridSpacing, gridSpacing * i, 0) + gridOffset);
        }
        
        Handles.color = Color.white;
    }

    private void DrawDraggedLine()
    {
        if (_currentDungeonGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(_currentDungeonGraph.roomNodeToDrawLineFrom.rect.center, _currentDungeonGraph.linePosition,
                _currentDungeonGraph.roomNodeToDrawLineFrom.rect.center, _currentDungeonGraph.linePosition,
                Color.white, null, connectingLineWidth);
        }
    }

    private void DrawRoomNodeConnections()
    {
        foreach (RoomNode roomNode in _currentDungeonGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    if (_currentDungeonGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, _currentDungeonGraph.roomNodeDictionary[childRoomNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNode parentRoomNode, RoomNode childRoomNode)
    {
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;
        
        Vector2 midPosition = (startPosition + endPosition) / 2f;
        Vector2 direction = (endPosition - startPosition);
        
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;
        
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1,
            Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2,
            Color.white, null, connectingLineWidth);
        
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition,
            Color.white, null, connectingLineWidth);
        
        GUI.changed = true;
    }

    private void DrawRoomNodes()
    {
        foreach (RoomNode roomNode in _currentDungeonGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    private void ProcessEvents(Event currentEvent)
    {
        graphDrag = Vector2.zero;
        
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        if (currentRoomNode == null || _currentDungeonGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    private RoomNode IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = _currentDungeonGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (_currentDungeonGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return _currentDungeonGraph.roomNodeList[i];
            }
        }
        return null;
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && _currentDungeonGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNode roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                if (_currentDungeonGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeIDToRoomNode(_currentDungeonGraph.roomNodeToDrawLineFrom.id);
                }
            }
            
            ClearLineDrag();
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (_currentDungeonGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void ProcessLeftMouseDragEvent(Vector2 delta)
    {
        graphDrag = delta;

        for (int i = 0; i < _currentDungeonGraph.roomNodeList.Count; i++)
        {
            _currentDungeonGraph.roomNodeList[i].DragNode(delta);
        }
        
        GUI.changed = true;
    }

    private void DragConnectingLine(Vector2 delta)
    {
        _currentDungeonGraph.linePosition += delta;
    }

    private void ClearLineDrag()
    {
        _currentDungeonGraph.roomNodeToDrawLineFrom = null;
        _currentDungeonGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNode roomNode in _currentDungeonGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);
        
        menu.ShowAsContext();
    }

    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (RoomNode roomNode in _currentDungeonGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNode childRoomNode = _currentDungeonGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        
        ClearAllSelectedRoomNodes();
    }

    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNode> roomNodesToDelete = new Queue<RoomNode>();

        foreach (RoomNode roomNode in _currentDungeonGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodesToDelete.Enqueue(roomNode);

                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    RoomNode childRoomNode = _currentDungeonGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    RoomNode parentRoomNode = _currentDungeonGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        while (roomNodesToDelete.Count > 0)
        {
            RoomNode roomNodeToDelete = roomNodesToDelete.Dequeue();
            
            _currentDungeonGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            _currentDungeonGraph.roomNodeList.Remove(roomNodeToDelete);
            
            DestroyImmediate(roomNodeToDelete, true);
            AssetDatabase.SaveAssets();
        }
    }

    private void CreateRoomNode(object obj)
    {
        if (_currentDungeonGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }
        CreateRoomNode(obj, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object obj, RoomNodeType roomNodeType)
    {
        Vector2 mousePosition = (Vector2)obj;
        
        RoomNode roomNode = ScriptableObject.CreateInstance<RoomNode>();
        _currentDungeonGraph.roomNodeList.Add(roomNode);
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), _currentDungeonGraph, roomNodeType);
        
        AssetDatabase.AddObjectToAsset(roomNode, _currentDungeonGraph);
        AssetDatabase.SaveAssets();
        
        _currentDungeonGraph.OnValidate();
    }

    private void SelectAllRoomNodes()
    {
        foreach (RoomNode roomNode in _currentDungeonGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        DungeonGraph dungeonGraph = EditorUtility.InstanceIDToObject(instanceID) as DungeonGraph;
        if (dungeonGraph != null)
        {
            OpenWindow();
            _currentDungeonGraph = dungeonGraph;
            return true;
        }
        return false;
    }

    private void InspectorSelectionChanged()
    {
        DungeonGraph dungeonGraph = Selection.activeObject as DungeonGraph;

        if (dungeonGraph != null)
        {
            _currentDungeonGraph = dungeonGraph;
            GUI.changed = true;
        }
    }

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;
        
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }
}
