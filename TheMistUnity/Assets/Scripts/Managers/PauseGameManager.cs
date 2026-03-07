using UnityEngine;

public class PauseGameManager : Singleton<PauseGameManager>
{
    [Header("Config")]
    [SerializeField] private GameObject pauseMenu;
    
    private PlayerActions actions;

    public bool isPaused;
    
    UIManager uIManager;
    SaveLoadManager saveLoadManager;
    CombatManager combatManager;
    GameManager gameManager;
    AudioManager audioManager;
    DialogueManager dialogueManager;
    
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
        
        uIManager = UIManager.Instance;
        saveLoadManager = SaveLoadManager.Instance;
        combatManager = CombatManager.Instance;
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
        dialogueManager = DialogueManager.Instance;
    }

    public void TogglePauseMenu()
    {
        if (uIManager.IsInMenu()) return;
        if(!saveLoadManager.GameIsActive()) return;
        if (uIManager.IsPlayerDead()) return;
        if(combatManager.isFighting) return;
        
        uIManager.CloseAllPanels();
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        isPaused = pauseMenu.activeSelf;
        
        if (pauseMenu.activeSelf)
        {
            uIManager.HideGameHUD();
            gameManager.DisablePlayerMovement();
            audioManager.PlayMenuMusic();
        }
        else
        {
            uIManager.ShowGameHUD();
            gameManager.EnablePlayerMovement();
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
        dialogueManager.DisableDialogueActions();
        gameManager.DisablePlayerMovement();
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void UnPause()
    {
        dialogueManager.EnableDialogueActions();
        pauseMenu.SetActive(false);
        gameManager.EnablePlayerMovement();
        Time.timeScale = 1f;
        isPaused = false;
    }
    
    private void OnEnable()
    {
        if (actions != null)
        {
            actions.General.Pause.Enable();
        }
    }

    private void OnDisable()
    {
        if (actions != null)
        {
            actions.General.Pause.Disable();
        }
    }
    
}