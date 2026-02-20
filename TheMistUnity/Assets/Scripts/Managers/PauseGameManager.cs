using UnityEngine;

public class PauseGameManager : Singleton<PauseGameManager>
{
    [Header("Config")]
    [SerializeField] private GameObject pauseMenu;
    
    private PlayerActions actions;

    public bool isPaused;
    
    protected override void Awake()
    {
        base.Awake();
        if (actions == null)
        {
            actions = new PlayerActions();
        }
    }
    
    private void Start()
    {
        actions.General.Pause.performed += ctx => TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        if(!SaveLoadManager.Instance.GameIsActive()) return;
        if (UIManager.Instance.IsPlayerDead()) return;
        
        UIManager.Instance.CloseAllPanels();
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        isPaused = pauseMenu.activeSelf;
        
        if (pauseMenu.activeSelf)
        {
            UIManager.Instance.HideGameHUD();
            GameManager.Instance.DisablePlayerMovement();
            AudioManager.Instance.PlayMenuMusic();
        }
        else
        {
            UIManager.Instance.ShowGameHUD();
            GameManager.Instance.EnablePlayerMovement();
            //TODO AudioManager.Instance.LoadCurrentMusic();
        }

        Time.timeScale = pauseMenu.activeSelf ? 0f : 1f;
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        DialogueManager.Instance.DisableActions();
        GameManager.Instance.DisablePlayerMovement();
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void UnPause()
    {
        DialogueManager.Instance.EnableActions();
        pauseMenu.SetActive(false);
        GameManager.Instance.EnablePlayerMovement();
        Time.timeScale = 1f;
        isPaused = false;
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