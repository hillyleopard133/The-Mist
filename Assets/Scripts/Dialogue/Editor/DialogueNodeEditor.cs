using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(DialogueNode))]
public class DialogueNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueNode node = (DialogueNode)target;
        
        // Fetch parent Dialogue asset
        Dialogue dialogue = GetParentDialogue(node);
        List<string> speakerOptions = new List<string>();

        if (dialogue != null)
        {
            speakerOptions = dialogue.GetCharacterNames().ToList(); // Fetch speakers from dialogue
        }

        // Ensure the stored index is within bounds
        if (node.speakerIndex < 0 || node.speakerIndex >= speakerOptions.Count)
        {
            node.speakerIndex = 0;
        }

        // Draw speaker dropdown
        node.speakerIndex = EditorGUILayout.Popup("Speaker", node.speakerIndex, speakerOptions.ToArray());
        
        EditorUtility.SetDirty(node);

        // Draw the rest of the inspector
        DrawDefaultInspector();
    }

    private Dialogue GetParentDialogue(DialogueNode node)
    {
        string assetPath = AssetDatabase.GetAssetPath(node);
        if (string.IsNullOrEmpty(assetPath)) return null;

        Dialogue parentDialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(assetPath);
        return parentDialogue;
    }
}