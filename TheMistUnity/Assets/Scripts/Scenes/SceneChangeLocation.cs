using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SceneChangeLocation", fileName = "SceneChangeLocation")]
public class SceneChangeLocation : ScriptableObject
{
    [Header("Info")]
    public string SceneEntryPointName;
    public string TargetScene;
    public AudioClip TargetSceneMusic;
    public float MusicVolume;
}