using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    
    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject deathScreenContent;
    [SerializeField] private Image respawnClock;
    [SerializeField] private float timeToRespawn;
    
    [Header("Tab Menu")]
    [SerializeField] private GameObject tabMenu;
    [SerializeField] private GameObject[] tabs;
    [SerializeField] private Button[] tabButtons;
    private int currentTab = 0;
    private const int inventoryTabNumber = 1;
    private const int equipmentTabNumber = 2;
    private const int questTabNumber = 3;
    private readonly Color selectedTabColor = new Color32(255,194,100, 255);
    
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
    private readonly Color completedTaskColor = new Color32(91,89,89, 255);
    private readonly Color currentTaskColor = new Color32(255,219,69, 255);
    private Quest currentlySelectedQuest;
    
    [Header("Inventory")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameTMP;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;
    [SerializeField] private InventorySlot inventorySlotPrefab;
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private Button[] inventoryTabs;
    [SerializeField] private Button destroyButton;
    public InventorySlot CurrentSlot { get; set; }
    private List<InventorySlot> slotList = new List<InventorySlot>(); 
    private readonly Color selectedInventoryColor = new Color32(106,214,0, 255);
    private int currentInventory = 0;
    private const int questInventoryNumber = 4;
    
    [Header("Equipment")]
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponAttackDamage;
    [SerializeField] private TextMeshProUGUI weaponCritHit;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI armourName;
    [SerializeField] private TextMeshProUGUI armourDefence;
    [SerializeField] private TextMeshProUGUI armourHealth;
    [SerializeField] private Image armourIcon;
    [SerializeField] private TextMeshProUGUI scrollName;
    [SerializeField] private TextMeshProUGUI scrollMana;
    [SerializeField] private Image scrollIcon;
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemStat1;
    [SerializeField] private TextMeshProUGUI selectedItemStat2;
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unEquipButton;
    [SerializeField] private EquipmentSlot equipmentSlotPrefab;
    [SerializeField] private Transform equipmentInventoryContainer;
    [SerializeField] private Sprite[] characterIcons;
    public EquipmentSlot CurrentEquipmentSlot { get; set; }
    private List<EquipmentSlot> equipmentSlotList = new List<EquipmentSlot>();
    private int currentEquipment = 0;
    
    [Header("Party")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterDescription;
    [SerializeField] private TextMeshProUGUI characterHealth;
    [SerializeField] private TextMeshProUGUI characterDefence;
    [SerializeField] private TextMeshProUGUI characterAttack;
    [SerializeField] private TextMeshProUGUI characterCritChance;
    [SerializeField] private TextMeshProUGUI characterMana;

    [SerializeField] private Button[] partyMembers;
    [SerializeField] private Image[] partyMemberImages;
    [SerializeField] private GameObject[] questionMarks;
    private int selectedPartyMember = 0;
    
    private float respawnTimer;
    private PlayerActions actions;
    private bool isReviving;

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
        
        actions.UI.LeftTab.performed += ctx => SwitchTab(-1);
        actions.UI.RightTab.performed += ctx => SwitchTab(1);
        
        actions.UI.LeftInv.performed += ctx => SwitchInventory(-1);
        actions.UI.RightInv.performed += ctx => SwitchInventory(1);
        
        InitialiseInventory();
        InitialiseEquipmentInventory();
        VerifyItemsForDraw();
        VerifyEquipmentItemsForDraw();
    }
    
    private void Update()
    {
        UpdatePlayerUI();
        UpdateRevivingClock();
    }

    public void UnlockPartyMember(int index)
    {
        partyMembers[index].interactable = true;
        partyMemberImages[index].color = Color.white;
        questionMarks[index - 1].SetActive(false);
    }

    public void ResetPartyUnlocks()
    {
        partyMembers[1].interactable = false;
        partyMembers[2].interactable = false;
        partyMemberImages[1].color = Color.black;
        partyMemberImages[2].color = Color.black;
        questionMarks[0].SetActive(true);
        questionMarks[1].SetActive(true);
    }

    private void UpdateEquippedItems()
    {
        InventoryItem[] equippedItems = EquipmentManager.Instance.GetCharacterEquipment(selectedPartyMember);
        ItemArmour armour = (ItemArmour) equippedItems[0];
        ItemWeapon weapon = (ItemWeapon) equippedItems[1];
        ItemScroll scroll = (ItemScroll) equippedItems[2];

        if (armour != null)
        {
            armourName.text = armour.Name;
            armourIcon.gameObject.SetActive(true);
            armourIcon.sprite = armour.Icon;
            armourHealth.text = "Health: " + armour.health;
            armourDefence.text = "Defence: " + armour.defence;
        }
        else
        {
            armourName.text = "No Armour Equipped";
            armourIcon.gameObject.SetActive(false);
            armourHealth.text = "";
            armourDefence.text = "";
        }

        if (weapon != null)
        {
            weaponName.text = weapon.Name;
            weaponIcon.gameObject.SetActive(true);
            weaponIcon.sprite = weapon.Icon;
            weaponAttackDamage.text = "Damage: " + weapon.damage;
            weaponCritHit.text = "Crit Chance: " + weapon.critChance + "%";
        }
        else
        {
            weaponName.text = "No Weapon Equipped";
            weaponIcon.gameObject.SetActive(false);
            weaponAttackDamage.text = "";
            weaponCritHit.text = "";
        }

        if (scroll != null)
        {
            scrollName.text = scroll.Name;
            scrollIcon.gameObject.SetActive(true);
            scrollIcon.sprite = scroll.Icon;
            scrollMana.text = "Mana: " + scroll.mana;
        }
        else
        {
            scrollName.text = "No Scroll Equipped";
            scrollIcon.gameObject.SetActive(false);
            scrollMana.text = "";
        }
        
    }

    public void FilterEquipment(int index)
    {
        InventoryItem[] items = EquipmentManager.Instance.SortEquipment(index);
        currentEquipment = index;
        DrawEquipmentInventory(items);
        
        int currentEquipped = EquipmentManager.Instance.GetEquippedSlotIndex(index, selectedPartyMember);
        if (currentEquipped != -1)
        {
            CurrentEquipmentSlot = equipmentSlotList[currentEquipped];
            ShowSelectedEquipment(CurrentEquipmentSlot.Index);
        }
        else
        {
            ShowSelectedEquipment(0);
        }
    }

    private void UpdateEquipmentList()
    {
        InventoryItem[] items = EquipmentManager.Instance.SortEquipment(currentEquipment);
        DrawEquipmentInventory(items);
        UpdateEquippedItems();
    }

    public void EquipSelectedItem()
    {
        if (CurrentEquipmentSlot == null)
        {
            return;
        }
        EquipmentManager.Instance.EquipItem(CurrentEquipmentSlot.Index, selectedPartyMember);
        UpdateEquipmentList();
    }

    public void UnEquipSelectedItem()
    {
        if (CurrentEquipmentSlot == null)
        {
            return;
        }
        EquipmentManager.Instance.UnequipItem(CurrentEquipmentSlot.Index, selectedPartyMember);
        UpdateEquipmentList();
    }
    
    private void ShowSelectedEquipment(int index)
    {
        InventoryItem item = EquipmentManager.Instance.SortEquipment(currentEquipment)[index];

        if (item == null)
        {
            selectedItemIcon.gameObject.SetActive(false);
            selectedItemName.text = "No Item Selected";
            selectedItemStat1.text = "";
            selectedItemStat2.text = "";
            return;
        }
        
        selectedItemIcon.gameObject.SetActive(true);
        selectedItemIcon.sprite = item.Icon;
        selectedItemName.text = item.Name;

        if (item is ItemArmour armour)
        {
            selectedItemStat1.text = "Health: " + armour.health;
            selectedItemStat2.text = "Defence: " + armour.defence;
        }
        else if (item is ItemWeapon weapon)
        {
            selectedItemStat1.text = "Damage: " + weapon.damage;
            selectedItemStat2.text = "Crit Chance: " + weapon.critChance + "%";
        }
        else if (item is ItemScroll scroll)
        {
            selectedItemStat1.text = "Mana: " + scroll.mana;
            selectedItemStat2.text = "";
        }
    }

    private void SwitchInventory(int direction)
    {
        currentInventory += direction;
        if(currentInventory < 0) currentInventory = inventoryTabs.Length - 1;
        if(currentInventory >= inventoryTabs.Length) currentInventory = 0;
        
        SelectInventory(currentInventory);
    }

    public void SelectInventory(int index)
    {
        Inventory.Instance.SelectInventory(index);
        currentInventory = index;
        DrawInventory(Inventory.Instance.GetCurrentInventory());
        
        foreach (Button tab in inventoryTabs)
        {
            ColorBlock colors = tab.colors;    
            colors.normalColor = Color.white;
            tab.colors = colors; 
        }
        
        Button SelectedTab = inventoryTabs[index];
        ColorBlock cb = SelectedTab.colors;    
        cb.normalColor = selectedInventoryColor;
        SelectedTab.colors = cb;

        ShowItemDescription(0);
        if (index == questInventoryNumber)
        {
            destroyButton.interactable = false;
        }
        else
        {
            destroyButton.interactable = true;
        }
    }
    
    private void VerifyItemsForDraw()
    {
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            if (Inventory.Instance.GetCurrentInventory()[i] == null)
            {
                DrawItem(null, i);
            }
        }
    }
    
    private void VerifyEquipmentItemsForDraw()
    {
        for (int i = 0; i < EquipmentManager.Instance.inventorySize; i++)
        {
            if (EquipmentManager.Instance.SortEquipment(currentEquipment)[i] == null)
            {
                DrawEquipmentItem(null, i);
            }
        }
    }
    
    private void DrawInventory(InventoryItem[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            DrawItem(items[i], i);
        }

        for (int i = items.Length; i < slotList.Count; i++)
        {
            DrawItem(null, i);
        }
    }
    
    private void DrawEquipmentInventory(InventoryItem[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            DrawEquipmentItem(items[i], i);
        }

        for (int i = items.Length; i < equipmentSlotList.Count; i++)
        {
            DrawEquipmentItem(null, i);
        }
    }
    
    private void InitialiseInventory()
    {
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            InventorySlot slot = Instantiate(inventorySlotPrefab, inventoryContainer);
            slot.Index = i;
            slotList.Add(slot);
        }
    }
    
    private void InitialiseEquipmentInventory()
    {
        for (int i = 0; i < EquipmentManager.Instance.inventorySize; i++)
        {
            EquipmentSlot slot = Instantiate(equipmentSlotPrefab, equipmentInventoryContainer);
            slot.Index = i;
            equipmentSlotList.Add(slot);
        }
    }
    
    public void RemoveItem()
    {
        if (CurrentSlot == null)
        {
            return;
        }

        Inventory.Instance.RemoveItem(Inventory.Instance.GetCurrentInventory(),CurrentSlot.Index);
    }
    
    public void DrawItem(InventoryItem item, int index)
    {
        InventorySlot slot = slotList[index];
        if (item == null)
        {
            slot.ShowSlotInformation(false);
            return;
        }
        slot.ShowSlotInformation(true);
        slot.UpdateSlot(item);
    }
    
    public void DrawEquipmentItem(InventoryItem item, int index)
    {
        EquipmentSlot slot = equipmentSlotList[index];
        if (item == null)
        {
            slot.ShowSlotInformation(false);
            return;
        }
        slot.ShowSlotInformation(true);

        if (item is ItemArmour armour)
        {
            if (armour.equipped != -1)
            {
                slot.UpdateSlot(item, characterIcons[armour.equipped]);
            }
            else
            {
                slot.UpdateSlot(item, null);
            }
        }
        else if (item is ItemWeapon weapon)
        {
            if (weapon.equipped != -1)
            {
                slot.UpdateSlot(item, characterIcons[weapon.equipped]);
            }
            else
            {
                slot.UpdateSlot(item, null);
            }
        }
        else if (item is ItemScroll scroll)
        {
            if (scroll.equipped != -1)
            {
                slot.UpdateSlot(item, characterIcons[scroll.equipped]);
            }
            else
            {
                slot.UpdateSlot(item, null);
            }
        }
        else slot.UpdateSlot(item, null);
    }
    
    private void ShowItemDescription(int index)
    {
        InventoryItem[] items = Inventory.Instance.GetCurrentInventory();

        if (items[index] == null)
        {
            itemIcon.sprite = null;
            itemNameTMP.text = "Item Name";
            itemDescriptionTMP.text = "Item Description";
        }
        else
        {
            itemIcon.sprite = items[index].Icon;
            itemNameTMP.text = items[index].Name;
            itemDescriptionTMP.text = items[index].Description;
        }
    }
    
    private void SlotSelectedCallback(int slotIndex)
    {
        CurrentSlot = slotList[slotIndex];
        ShowItemDescription(slotIndex);
    }

    private void EquipmentSlotSelectedCallback(int slotIndex)
    {
        CurrentEquipmentSlot = equipmentSlotList[slotIndex];
        ShowSelectedEquipment(slotIndex);
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

        foreach (Button tab in tabButtons)
        {
            ColorBlock colors = tab.colors;    
            colors.normalColor = Color.white;
            tab.colors = colors; 
        }
        
        tabs[tabIndex].SetActive(true);
        currentTab = tabIndex;

        if (currentTab == questTabNumber && currentlySelectedQuest != null)
        {
            LoadQuestsUI();
            SelectQuest(currentlySelectedQuest);
        }

        if (currentTab == equipmentTabNumber)
        {
            FilterEquipment(0);
            UpdateEquippedItems();
        }
        
        if(currentTab == inventoryTabNumber) DrawInventory(Inventory.Instance.GetCurrentInventory());
        
        Button SelectedTab = tabButtons[tabIndex];
        ColorBlock cb = SelectedTab.colors;    
        cb.normalColor = selectedTabColor;
        SelectedTab.colors = cb;   
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
        InventorySlot.OnSlotSelectedEvent += SlotSelectedCallback;
        EquipmentSlot.OnSlotSelectedEvent += EquipmentSlotSelectedCallback;
    }

    private void OnDisable()
    {
        if (Instance != this) return;
        
        actions.General.Enable();
        actions.UI.Disable();
        
        PlayerUpgrade.OnPlayerUpgradeEvent -= UpgradeCallback;
        DialogueManager.OnExtraInteractionEvent -= ExtraInteractionCallback;
        InventorySlot.OnSlotSelectedEvent -= SlotSelectedCallback;
        EquipmentSlot.OnSlotSelectedEvent -= EquipmentSlotSelectedCallback;
    }
    
}
