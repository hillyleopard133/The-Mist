using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/CharacterSpeaker")]
public class CharacterSpeaker : ScriptableObject
{
    [Header("Info")]
    public string Name;
    public Sprite Icon;

}