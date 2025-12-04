using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Trigger")]
public class DialogueTrigger : ScriptableObject
{
    [TextArea] [SerializeField] private string triggerDescription;
}