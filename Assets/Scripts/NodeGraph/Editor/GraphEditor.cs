using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

public class GraphEditor : EditorWindow
{
    private Graph selectedGraph = null;
    
    [NonSerialized] private GraphNode draggingNode = null;
    [NonSerialized] private GraphNode nodeToCreate = null;
    [NonSerialized] private GraphNode nodeToDelete = null;
    [NonSerialized] private GraphNode nodeToLink = null;
    
    private const float canvasWidth = 4000f;
    private const float canvasHeight = 4000f;
    private const float backgroundSize = 50f;

    //Graph and canvas space 
    [NonSerialized] private Vector2 scrollViewOffset;
    [NonSerialized] private Vector2 nodeDragOffset;
    [NonSerialized] private bool isDraggingCanvas;
    private Vector2 scrollPosition;
    
    //Node style
    private GUIStyle NodeStyle1;
    private GUIStyle NodeStyle2;
    
    [MenuItem("Graph Editor", menuItem = "Window/Graph Editor")]
    
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(GraphEditor), false, "Graph Editor");
    }
    
    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Graph graph = EditorUtility.InstanceIDToObject(instanceID) as Graph;
        if(graph == null) return false;
            
        ShowEditorWindow();
        return true;
    }
    
    private void OnGUI()
    {
        if (selectedGraph == null) return;

        ProcessEvents();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        Rect canvas = GUILayoutUtility.GetRect(canvasWidth, canvasHeight);
        Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
        Rect textureCoords = new Rect(0, 0, canvas.width/backgroundSize, canvas.height/backgroundSize);
        GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);
            
        foreach (GraphNode node in selectedGraph.GetAllNodes())
        {
            DrawConnections(node);
        }
        foreach (GraphNode node in selectedGraph.GetAllNodes())
        {
            DrawNode(node);
        }

        EditorGUILayout.EndScrollView();

        if (nodeToCreate != null)
        {
            selectedGraph.CreateNode(nodeToCreate);
            nodeToCreate = null;
        }

        if (nodeToDelete != null)
        {
            selectedGraph.DeleteNode(nodeToDelete);
            nodeToDelete = null;
        }
    }
    
    private void DrawNode(GraphNode node)
    {
        GUIStyle style = NodeStyle1;
        if (node.IsNodeStyle2())
        {
            style = NodeStyle2;
        }
        GUILayout.BeginArea(node.GetRect(), style);
            
        EditorGUILayout.LabelField("Node");

        node.DrawNode();
            
        DrawButtons(node);
            
        GUILayout.EndArea();
    }

    private void ProcessEvents()
    { 
        if (draggingNode == null)
        {
            if (Event.current.type == EventType.MouseDown)
            { 
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    nodeDragOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    isDraggingCanvas = true;
                    scrollViewOffset = scrollPosition + Event.current.mousePosition;
                    Selection.activeObject = selectedGraph;
                }
            }
        }
        else
        {
            isDraggingCanvas = false;
            if(Event.current.type == EventType.MouseUp)
            {
                draggingNode = null;
                    
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                draggingNode.SetPosition(Event.current.mousePosition + nodeDragOffset);
                GUI.changed = true;
            }
                
        }
        if (isDraggingCanvas)
        {
            if (Event.current.type == EventType.MouseUp)
            {
                isDraggingCanvas = false;
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                scrollPosition = scrollViewOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
        }

    }

    private GraphNode GetNodeAtPoint(Vector2 point)
    {
        GraphNode foundNode = null;
        foreach (GraphNode node in selectedGraph.GetAllNodes())
        {
            if (node.GetRect().Contains(point))
            {
                foundNode =  node;
            }
        }
            
        return foundNode;
    }
    
    private void DrawButtons(GraphNode node)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Delete"))
        {
            nodeToDelete = node;
        }

        if (nodeToLink == null)
        {
            if (GUILayout.Button("Link"))
            {
                nodeToLink = node;
            }
        }
        else if (nodeToLink == node)
        {
            if (GUILayout.Button("Cancel"))
            {
                nodeToLink = null;
            }
        }
        else
        {
            if (nodeToLink.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Remove child"))
                {
                    nodeToLink.RemoveChild(node.name);
                    nodeToLink = null;
                }
            }
            else
            {
                if (GUILayout.Button("Add child"))
                {
                    nodeToLink.AddChild(node.name);
                    nodeToLink = null;
                }
            }
        }

        if (GUILayout.Button("Add"))
        {
            nodeToCreate = node;
        }

        GUILayout.EndHorizontal();
    }

    private void DrawConnections(GraphNode node)
    {
        foreach (GraphNode childNode in selectedGraph.GetAllChildren(node))
        {
            DrawConnectionLine(node, childNode);
        }
    }
    
    private void DrawConnectionLine(GraphNode parentNode, GraphNode childNode)
    {
        Vector2 startPosition = parentNode.GetRect().center;
        Vector2 endPosition = childNode.GetRect().center;

        Vector2 direction = endPosition - startPosition;
        Vector2 midPosition = (startPosition + endPosition) / 2f;

        float arrowSize = 10f; 

        Vector2 perpendicular = new Vector2(-direction.y, direction.x).normalized * arrowSize;
        Vector2 arrowTail1 = midPosition - perpendicular;
        Vector2 arrowTail2 = midPosition + perpendicular;
        Vector2 arrowHead = midPosition + direction.normalized * arrowSize;

        Handles.DrawLine(arrowHead, arrowTail1);
        Handles.DrawLine(arrowHead, arrowTail2);

        Handles.DrawLine(startPosition, endPosition);

        GUI.changed = true;
    }

    private void OnSelectionChanged()
    {
        Graph newGraph = Selection.activeObject as Graph;
        selectedGraph = newGraph == null ? selectedGraph : newGraph;
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;

        selectedGraph = Selection.activeObject as Graph;

        NodeStyle1 = new GUIStyle();
        NodeStyle1.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
        NodeStyle1.padding = new RectOffset(20, 20, 20, 20);
        NodeStyle1.border = new RectOffset(12, 12, 12, 12);

        NodeStyle2 = new GUIStyle();
        NodeStyle2.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        NodeStyle2.padding = new RectOffset(20, 20, 20, 20);
        NodeStyle2.border = new RectOffset(12, 12, 12, 12);
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    
}
