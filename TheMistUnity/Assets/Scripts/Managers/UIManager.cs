using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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
    
    [Header("Extra Panels")]
    [SerializeField] private GameObject npcQuestPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject craftingPanel;
    
    [Header("Start Menu")]
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject newGameWarning;
    [SerializeField] private Button loadButton;
    
    [Header("Options Menu")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject playerControlsPanel;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Enemy Info Panel")] 
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private Image enemyIcon;
    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private TextMeshProUGUI enemyName;
    [SerializeField] private TextMeshProUGUI enemyHealth;
    [SerializeField] private TextMeshProUGUI enemyExp;
    [SerializeField] private TextMeshProUGUI enemyDamage;
    [SerializeField] private Transform enemyInfoLootContainer;
    [SerializeField] private EnemyInfoLootItem enemyInfoLootItemPrefab;
    [SerializeField] private ScrollRect enemyInfoScrollRect;
    
    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject deathScreenContent;
    [SerializeField] private Image respawnClock;
    [SerializeField] private float timeToRespawn;
    
    [Header("Tab Menu")]
    [SerializeField] private GameObject tabMenu;
    [SerializeField] private GameObject[] tabs;
    [SerializeField] private GameObject[] tabButtons;
    
    [Header("Quests")]
    [SerializeField] private TextMeshProUGUI mainQuestTitle;
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private GameObject taskList;
    [SerializeField] private TextMeshProUGUI questGiverName;
    [SerializeField] private Image questGiverIcon;
    [SerializeField] private GameObject taskPrefab;

    [SerializeField] private GameObject sideQuestList;
    [SerializeField] private GameObject questPrefab;
    
    private Color completedTaskColor = new Color32(91,89,89, 255);
    private Color currentTaskColor = new Color32(255,219,69, 255);

    private int currentTab = 0;
    private int questTabNumber = 3;
    
    private float respawnTimer;
    private PlayerActions actions;
    private bool isReviving;

    private EnemyBrain enemyInPanel;

    private Quest currentlySelectedQuest;

    private void Awake()
    {
        base.Awake();
        actions = new PlayerActions();
    }

    private void Start()
    {
        actions.General.Respawn.performed += ctx => SetIsReviving(true);  
        actions.General.Respawn.canceled += ctx => SetIsReviving(false); 
        actions.General.TabMenu.performed += ctx => OpenCloseTabMenu();
        
        actions.UI.Left.performed += ctx => SwitchTab(-1);
        actions.UI.Right.performed += ctx => SwitchTab(1);
    }
    
    private void Update()
    {
        UpdatePlayerUI();
        UpdateRevivingClock();
    }

    public void LoadQuestsUI()
    {
        UpdateSideQuestList();
    }

    public void UpdateSideQuestList()
    {
        foreach (Transform child in sideQuestList.transform)
        {
            Destroy(child.gameObject);
        }

        List<Quest> questList = QuestManager.Instance.acceptedQuests;
        foreach (Quest quest in questList)
        {
            if(quest.QuestCompleted || quest.IsMainQuest) continue;
            
            GameObject newQuest = Instantiate(questPrefab, sideQuestList.transform);
            newQuest.GetComponent<Button>().onClick.AddListener(() => SelectQuest(quest));
        }
    }

    private void SelectQuest(Quest quest)
    {
        currentlySelectedQuest = quest;
        
        questTitle.text = quest.Name;
        questDescription.text = quest.Description;
        questGiverIcon.sprite = quest.QuestGiverIcon;
        questGiverName.text = quest.QuestGiverName;

        foreach (Transform child in taskList.transform)
        {
            Destroy(child.gameObject);
        }

        bool currentTaskReached = false;
        TextMeshProUGUI newTaskText = null;
        foreach (QuestTask task in quest.Tasks)
        {
            if (!currentTaskReached)
            {
                GameObject newTask = Instantiate(taskPrefab, taskList.transform);
                newTaskText = newTask.GetComponentInChildren<TextMeshProUGUI>();
                newTaskText.text = task.GetDetails();
                newTaskText.color = completedTaskColor;
            }
            if (!task.IsCompleted)
            {
                if (newTaskText != null)
                {
                    newTaskText.color = currentTaskColor;
                }
                currentTaskReached = true;
            }
        }
    }
    
    private void OpenCloseTabMenu()
    {
        if (!SaveLoadManager.Instance.GameIsActive()) return;
        
        bool opening = !tabMenu.activeSelf;
        
        if(opening && PauseGameManager.Instance.isPaused) return;
        
        tabMenu.SetActive(opening);

        if (opening)
        {
            SetTabMenu(currentTab);
            actions.UI.Enable();
            PauseGameManager.Instance.PauseGame();
        }
        else
        {
            actions.UI.Disable();
            PauseGameManager.Instance.UnPause();
        }
    }

    public void SetTabMenu(int tabIndex)
    {
        foreach (GameObject tab in tabs)
        {
            tab.SetActive(false);
        }
        tabs[tabIndex].SetActive(true);
        currentTab = tabIndex;
        if(currentTab == questTabNumber && currentlySelectedQuest != null) SelectQuest(currentlySelectedQuest);
        EventSystem.current.SetSelectedGameObject(tabButtons[tabIndex].gameObject);
    }

    private void SwitchTab(int direction)
    {
        if (!tabMenu.activeSelf) return;
        
        currentTab += direction;
        if (currentTab < 0) currentTab = tabs.Length - 1;
        if(currentTab >= tabs.Length) currentTab = 0;
        SetTabMenu(currentTab);
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

    public bool NPCInteractionPanelOpen()
    {
        if(npcQuestPanel.activeSelf || shopPanel.activeSelf) return true;
        
        return false;
    }

    public void UpdateEnemyInfoPanel(EnemyBrain enemyBrain)
    {
        if (enemyBrain == null)
        {
            CloseEnemyInfoPanel();
            return;
        }
        CloseAllPanels();
        enemyInPanel = enemyBrain;
        enemyInfoScrollRect.verticalNormalizedPosition = 1f;
        enemyInfoPanel.SetActive(true);
        enemyIcon.sprite = enemyBrain.Icon;
        enemyName.text = enemyBrain.Name;
        float currentHealth = enemyBrain.GetComponent<EnemyHealth>().CurrentHealth;
        float maxHealth = enemyBrain.GetComponent<EnemyHealth>().health;
        if (currentHealth == Mathf.Floor(currentHealth))
        {
            enemyHealth.text = currentHealth.ToString("F0") + "/" + maxHealth; 
        }
        else
        {
            enemyHealth.text = currentHealth.ToString("F1") + "/" + maxHealth; 
        }
        enemyHealthBar.fillAmount = currentHealth / maxHealth;
        enemyExp.text = enemyBrain.GetComponent<EnemyLoot>().ExpDrop.ToString();
        enemyDamage.text = enemyBrain.GetComponent<ActionAttack>().damage.ToString();
        
        
        foreach (Transform child in enemyInfoLootContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DropItem item in enemyBrain.GetComponent<EnemyLoot>().dropItems)
        {
            EnemyInfoLootItem lootItem = Instantiate(enemyInfoLootItemPrefab, enemyInfoLootContainer);
            lootItem.ConfigLootInfo(item);
        }
    }

    public void EnemyInInfoPanelDamaged(EnemyBrain enemyBrain)
    {
        if (enemyBrain == null) return;
        if (enemyBrain != enemyInPanel) return;
        
        UpdateEnemyInfoPanel(enemyBrain);
    }

    public void CloseEnemyInfoPanel()
    {
        enemyInPanel = null;
        enemyInfoScrollRect.verticalNormalizedPosition = 1f;
        enemyInfoPanel.SetActive(false);
    }

    public bool IsPlayerDead()
    {
        return deathScreen.activeSelf;
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
    
    public void OpenOptionsPanel()
    {
        AudioManager.Instance.PlayButtonPressSound();
        optionsPanel.SetActive(true);
        if (PauseGameManager.Instance.isPaused)
        {
            PauseGameManager.Instance.HidePauseMenu();
        }
        else
        {
            HideStartMenu();
        }
    }

    public void CloseOptionsPanel()
    {
        AudioManager.Instance.PlayButtonPressSound();
        optionsPanel.SetActive(false);
        if (PauseGameManager.Instance.isPaused)
        {
            PauseGameManager.Instance.ShowPauseMenu();
        }
        else
        {
            ShowStartMenu();
        }
    }

    public void OpenPlayerControlsPanel()
    {
        AudioManager.Instance.PlayButtonPressSound();
        optionsPanel.SetActive(false);
        playerControlsPanel.SetActive(true);
    }

    public void ClosePlayerControlsPanel()
    {
        AudioManager.Instance.PlayButtonPressSound();
        optionsPanel.SetActive(true);
        playerControlsPanel.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1f;
        scrollRect.verticalScrollbar.value = 1f;
    }
    
    public void CloseAllPanels()
    {
        AudioManager.Instance.PlayButtonPressSound();
        CloseShopPanel();
        CloseCraftingPanel();
        CloseNPCQuestPanel();
        CloseEnemyInfoPanel();
        InventoryUI.Instance.CloseInventory();
        CloseStatsPanel();
        LootManager.Instance.ClosePanel();
        DialogueManager.Instance.CloseDialoguePanel();
    }

    private void CloseStatsPanel()
    {
        statsPanel.SetActive(false);
    }
    
    private void CloseCraftingPanel()
    {
        craftingPanel.SetActive(false);
    }

    private void CloseShopPanel()
    {
        shopPanel.SetActive(false);
    }

    private void CloseNPCQuestPanel()
    {
        npcQuestPanel.SetActive(false);
    }

    public void OpenCloseStatsPanel()
    {
        bool isActive = statsPanel.activeSelf;
        CloseAllPanels();
        statsPanel.SetActive(!isActive);
        if (isActive == false)
        {
            UpdateStatsPanel();
        }
    }

    public void OpenCloseNPCQuestPanel(bool value)
    {
        CloseAllPanels();
        npcQuestPanel.SetActive(value);
    }
    
    public void OpenCloseShopPanel(bool value)
    {
        CloseAllPanels();
        shopPanel.SetActive(value);
    }

    public void OpenCloseCraftingPanel(bool value)
    {
        CloseAllPanels();
        craftingPanel.SetActive(value);
        CraftingManager.Instance.HideRecipe();
    }
    
    private void UpdatePlayerUI()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, stats.Health / stats.MaxHealth, 10f * Time.deltaTime);
        manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, stats.Mana / stats.MaxMana, 10f * Time.deltaTime);
        expBar.fillAmount = Mathf.Lerp(expBar.fillAmount, stats.CurrentExp / stats.NextLevelExp, 10f * Time.deltaTime);
        
        levelTMP.text = $"Level {stats.Level}";
        healthTMP.text = $"{stats.Health} / {stats.MaxHealth}";
        manaTMP.text = $"{stats.Mana} / {stats.MaxMana}";
        expTMP.text = $"{stats.CurrentExp} / {stats.NextLevelExp}";
        coinsTMP.text = CoinManager.Instance.Coins.ToString();
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

    private void ExtraInteractionCallback(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Quest:
                OpenCloseNPCQuestPanel(true);
                break;
            case InteractionType.Shop:
                OpenCloseShopPanel(true);
                break;
            case InteractionType.Crafting:
                OpenCloseCraftingPanel(true);
                break;
        }
    }

    private void OnEnable()
    {
        if (Instance != this) return;
        
        actions.General.Enable();
        actions.UI.Disable();
        
        PlayerUpgrade.OnPlayerUpgradeEvent += UpgradeCallback;
        DialogueManager.OnExtraInteractionEvent += ExtraInteractionCallback;
    }

    private void OnDisable()
    {
        if (Instance != this) return;
        
        actions.General.Enable();
        actions.UI.Disable();
        
        PlayerUpgrade.OnPlayerUpgradeEvent -= UpgradeCallback;
        DialogueManager.OnExtraInteractionEvent -= ExtraInteractionCallback;
    }
    
}
