using UnityEngine;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private SceneChangeLocation location;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneChangeManager.Instance.LoadScene(location.TargetSceneName, location.SceneEntryPointName);
            if (location.TargetSceneMusic == null) return;
            AudioManager.Instance.PlayMusic(location.TargetSceneMusic, location.MusicVolume);
        }
    }
}