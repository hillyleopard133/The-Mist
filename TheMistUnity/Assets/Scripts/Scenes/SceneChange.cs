using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private SceneChangeLocation location;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneChangeManager.Instance.LoadScene(location.TargetScene, location.SceneEntryPointName);
            if (location.TargetSceneMusic == null) return;
            if (location.MusicHasIntro)
            {
                AudioManager.Instance.PlayMusicWithIntro(location.TargetSceneIntroMusic, location.TargetSceneMusic, location.MusicVolume);
            }
            else AudioManager.Instance.PlayMusic(location.TargetSceneMusic, location.MusicVolume);
        }
    }
}