using UnityEngine;

[CreateAssetMenu]
public class SceneChangeLocation : ScriptableObject
{
    [Header("Info")]
    public string SceneEntryPointName;
    public string TargetSceneName;
    public AudioClip TargetSceneMusic;
    public float MusicVolume;
}