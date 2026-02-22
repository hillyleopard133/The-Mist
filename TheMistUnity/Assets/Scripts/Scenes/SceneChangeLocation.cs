using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class SceneChangeLocation : ScriptableObject
{
    [Header("Info")]
    public string SceneEntryPointName;
    public SceneAsset TargetScene;
    public AudioClip TargetSceneMusic;
    public float MusicVolume;
}