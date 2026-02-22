using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleInteraction : MonoBehaviour
{
    [SerializeField] private GameObject interactionBox;
    [SerializeField] private SceneChangeLocation location;
    
    private PlayerActions actions;

    private void Awake()
    {
        if (actions == null)
        {
            actions = new PlayerActions();
        }
    }

    private void Start()
    {
        actions.Hamster.Interact.performed += ctx => EnterHole();
    }

    private void EnterHole()
    {
        if(!interactionBox.activeSelf || PauseGameManager.Instance.isPaused) return;
        
        SceneChangeManager.Instance.LoadScene(location.TargetScene.name, location.SceneEntryPointName);
        if (location.TargetSceneMusic == null) return;
        AudioManager.Instance.PlayMusic(location.TargetSceneMusic, location.MusicVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionBox.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactionBox.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        if (actions != null)
        {
            actions.Enable();
        }
    }

    private void OnDisable()
    {
        if (actions != null)
        {
            actions.Disable();
        }
    }
}
