using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

//namespaces group classes together and gives them access to each other.
//Children will be able to access the parents but not vice versa
//ie dialogue.editor can access dialogue but dialogue cannot access dialogue.editor

    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        [NonSerialized] private GUIStyle npcNodeStyle;
        [NonSerialized] private GUIStyle playerNodeStyle;
        [NonSerialized] private DialogueNode draggingNode = null;
        [NonSerialized] private Vector2 nodeDragOffset;
        [NonSerialized] private Vector2 scrollViewOffset;
        [NonSerialized] private bool isDraggingCanvas;
        [NonSerialized] private DialogueNode nodeToCreate = null;
        [NonSerialized] private DialogueNode nodeToDelete = null;
        [NonSerialized] private DialogueNode nodeToLink = null;
        private Vector2 scrollPosition;

        private const float canvasWidth = 4000f;
        private const float canvasHeight = 4000f;
        private const float backgroundSize = 50f;
        
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            //utility false means its a dockable window. true will be closable such as the sprite editor
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.EntityIdToObject(instanceID) as Dialogue;
            if(dialogue == null) return false;
            
            ShowEditorWindow();
            return true;
        }

        private void OnGUI()
        {
            if (selectedDialogue == null) return;

            ProcessEvents();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            Rect canvas = GUILayoutUtility.GetRect(canvasWidth, canvasHeight);
            Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
            Rect textureCoords = new Rect(0, 0, canvas.width/backgroundSize, canvas.height/backgroundSize);
            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);
            
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawNode(node);
            }
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node);
            }
            EditorGUILayout.EndScrollView();

            if (nodeToCreate != null)
            {
                selectedDialogue.CreateNode(nodeToCreate);
                nodeToCreate = null;
            }

            if (nodeToDelete != null)
            {
                selectedDialogue.DeleteNode(nodeToDelete);
                nodeToDelete = null;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = npcNodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);
            
            EditorGUILayout.LabelField("Node");
            
            //GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            //textAreaStyle.wordWrap = true;

            //node.SetText(EditorGUILayout.TextArea(node.GetText(), textAreaStyle,  GUILayout.ExpandHeight(true)));
            node.SetText(EditorGUILayout.TextField(node.GetText()));
            
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
                        Selection.activeObject = selectedDialogue;
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

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode =  node;
                }
            }
            
            return foundNode;
        }

        private void DrawButtons(DialogueNode node)
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
            else if(nodeToLink == node)
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
        
        
        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector3(node.GetRect().xMax, node.GetRect().center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector3(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 offset = endPosition - startPosition;
                offset.y = 0;
                offset.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition, startPosition + offset, endPosition - offset, 
                    Color.white, null, 5f);
            }
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            selectedDialogue = newDialogue == null ? selectedDialogue: newDialogue;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            
            selectedDialogue = Selection.activeObject as Dialogue;
            
            npcNodeStyle = new GUIStyle();
            npcNodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            npcNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            npcNodeStyle.border = new RectOffset(12,12,12,12);
            
            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12,12,12,12);
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }


