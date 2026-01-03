using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;

    [Header("Bars")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image expBar;
    
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI healthTMP;
    [SerializeField] private TextMeshProUGUI manaTMP;
    [SerializeField] private TextMeshProUGUI expTMP;
    [SerializeField] private TextMeshProUGUI coinsTMP;
    
    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI statsLevelTMP;
    [SerializeField] private TextMeshProUGUI statsDamageTMP;
    [SerializeField] private TextMeshProUGUI statsCChanceTMP;
    [SerializeField] private TextMeshProUGUI statsCDamageTMP;
    [SerializeField] private TextMeshProUGUI statsTotalExpTMP;
    [SerializeField] private TextMeshProUGUI statsCurrentExpTMP;
    [SerializeField] private TextMeshProUGUI statsReqExpTMP;
    
    [SerializeField] private TextMeshProUGUI attributePointsTMP;
    [SerializeField] private TextMeshProUGUI strengthTMP;
    [SerializeField] private TextMeshProUGUI dexterityTMP;
    [SerializeField] private TextMeshProUGUI intelligenceTMP;
    
    [Header("Start Menu")]
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject newGameWarning;
    [SerializeField] private Button loadButton;
    
    [Header("Options Menu")]
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject playerControlsPanel;
    [SerializeField] private ScrollRect scrollRect;
    
    
    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject deathScreenContent;
    [SerializeField] private Image respawnClock;
    [SerializeField] private float timeToRespawn;
    
    private float respawnTimer;
    private PlayerActions actions;
    private bool isReviving;

    protected override void Awake()
    {
        base.Awake();
        actions = new PlayerActions();
    }

    private void Start()
    {
        actions.General.Respawn.performed += ctx => SetIsReviving(true);  
        actions.General.Respawn.canceled += ctx => SetIsReviving(false); 
    }
    
    private void Update()
    {
        UpdateRevivingClock();
    }
    
    public bool IsPlayerDead()
    {
        return deathScreen.activeSelf;
    }


    public void DisableLoadButton()
    {
        loadButton.interactable = false;
    }

    public void EnableLoadButton()
    {
        loadButton.interactable = true;
    }

    public void ActivateNewGameWarning()
    {
        HideStartMenu();
        newGameWarning.SetActive(true);
    }

    public void CancelNewGame()
    {
        HideNewGameWarning();
        ShowStartMenu();
    }

    public void HideNewGameWarning()
    {
        newGameWarning.SetActive(false);
    }

    private void SetIsReviving(bool reviving)
    {
        isReviving = reviving;
    }

    private void UpdateRevivingClock()
    {
        if (deathScreenContent.activeSelf == false) return;
        if (isReviving && respawnTimer <= timeToRespawn)
        {
            respawnTimer += Time.deltaTime;
            
        }
        else if(respawnTimer >= 0)
        {
            respawnTimer -= Time.deltaTime;
        }
        
        respawnClock.fillAmount = respawnTimer / timeToRespawn;

        if (respawnClock.fillAmount >= 1)
        {
            PlayerRespawned();
            respawnTimer = 0;
            respawnClock.fillAmount = 0;
        }
    }
    
    
    public void ActivateDeathScreen()
    {
        HideGameHUD();
        CloseAllPanels();
        AudioManager.Instance.PlayDeadMusic();
        deathScreen.SetActive(true);
        deathScreen.GetComponent<Animator>().SetTrigger("Death");
    }

    public void PlayerRespawned()
    {
        AudioManager.Instance.LoadCurrentMusic();
        deathScreenContent.SetActive(false);
        deathScreen.GetComponent<Animator>().SetTrigger("Respawn");
        Player.Instance.RespawnPlayer();
    }

    public void ShowDeathScreenContent()
    {
        deathScreenContent.SetActive(true);
    }

    public void DeactivateDeathScreen()
    {
        deathScreen.SetActive(false);
        ShowGameHUD();
    }
    
    public void HideStartMenu()
    {
        startMenu.SetActive(false);
    }

    public void ShowStartMenu()
    {
        startMenu.SetActive(true);
    }

    public void HideGameHUD()
    {
        gameHUD.SetActive(false);
    }

    public void ShowGameHUD()
    {
        gameHUD.SetActive(true);
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }
    
    public void OpenPlayerControlsPanel()
    {
        AudioManager.Instance.PlayButtonPressSound();
        optionsMenu.SetActive(false);
        playerControlsPanel.SetActive(true);
    }

    public void ClosePlayerControlsPanel()
    {
        AudioManager.Instance.PlayButtonPressSound();
        optionsMenu.SetActive(true);
        playerControlsPanel.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1f;
        scrollRect.verticalScrollbar.value = 1f;
    }
    
    public void CloseAllPanels()
    {
        AudioManager.Instance.PlayButtonPressSound();
        InventoryUI.Instance.CloseInventory();
        CloseStatsPanel();
        DialogueManager.Instance.CloseDialoguePanel();
    }
    
    private void CloseStatsPanel()
    {
        statsPanel.SetActive(false);
    }
    
    private void UpdateStatsPanel()
    {
        statsLevelTMP.text = stats.Level.ToString();
        statsDamageTMP.text = stats.TotalDamage.ToString();
        statsCChanceTMP.text = stats.CriticalChance.ToString();
        statsCDamageTMP.text = stats.CriticalDamage.ToString();
        statsTotalExpTMP.text = stats.TotalExp.ToString();
        statsCurrentExpTMP.text = stats.CurrentExp.ToString();
        statsReqExpTMP.text = stats.NextLevelExp.ToString();

        attributePointsTMP.text = $"Points: {stats.AttributePoints}";
        strengthTMP.text = stats.Strength.ToString();
        dexterityTMP.text = stats.Dexterity.ToString();
        intelligenceTMP.text = stats.Intelligence.ToString();
    
    }

    private void UpgradeCallback()
    {
        UpdateStatsPanel();
    }
    
    private void OnEnable()
    {
        actions.Enable();
        PlayerUpgrade.OnPlayerUpgradeEvent += UpgradeCallback;
    }

    private void OnDisable()
    {
        PlayerUpgrade.OnPlayerUpgradeEvent -= UpgradeCallback;
        
    }


}
