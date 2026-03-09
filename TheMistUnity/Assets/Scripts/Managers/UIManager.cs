using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    
    [Header("Crafting")]
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
    private float respawnTimer;
    private bool isReviving;
    
    [Header("Timing Window")]
    [SerializeField] private GameObject timingWindowScreen;
    [SerializeField] private GameObject timingBar;
    [SerializeField] private GameObject timingWindow;
    [SerializeField] private GameObject timingSlider;
    [SerializeField] private int timingWindowBaseSize;
    private RectTransform sliderRect;
    private RectTransform barRect;
    private RectTransform windowRect;
    private bool isAttackTiming;
    private bool isTiming;
    
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private RectTransform loadingProgressBar;
    
    [Header("Tab Menu")]
    [SerializeField] private GameObject tabMenu;
    [SerializeField] private GameObject[] tabs;
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedTabColor;
    private int currentTab = 0;
    
    [Header("Quests")]
    [SerializeField] private TextMeshProUGUI mainQuestTitle;
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private GameObject taskList;
    [SerializeField] private TextMeshProUGUI questGiverName;
    [SerializeField] private Image questGiverIcon;
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private GameObject[] questHeaderPrefabs;
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Color completedTaskColor;
    [SerializeField] private Color currentTaskColor;
    [SerializeField] private GameObject questRewardsList;
    [SerializeField] private GameObject questRewardPrefab;
    [SerializeField] private Sprite coinIcon;
    [SerializeField] private Sprite expIcon;
    private Quest currentlySelectedQuest;
    
    [Header("Inventory")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameTMP;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;
    [SerializeField] private TextMeshProUGUI itemSellValue;
    [SerializeField] private InventorySlot inventorySlotPrefab;
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private Button[] inventoryTabs;
    [SerializeField] private Button destroyButton;
    [SerializeField] private Color selectedInventoryColor;
    [SerializeField] private TextMeshProUGUI playerCoinAmountInventory;
    private InventorySlot CurrentInventorySlot;
    private List<InventorySlot> inventorySlotList = new List<InventorySlot>(); 
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
    [SerializeField] private Color selectedEquipmentColor;
    [SerializeField] private Button[] equipmentCards;
    private EquipmentSlot CurrentEquipmentSlot;
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
    [SerializeField] private Button[] partyMembersButtons;
    [SerializeField] private Image[] partyMemberImages;
    [SerializeField] private GameObject[] questionMarks;
    [SerializeField] private GameObject[] partyMemberShrouds;
    [SerializeField] private Color selectedPartyMemberColor;
    private int selectedPartyMember = 0;
    
    [Header("Shop")]
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private TextMeshProUGUI shopHeading;
    [SerializeField] private Transform shopItemContainer;
    [SerializeField] private Transform playerShopItemContainer;
    [SerializeField] private TextMeshProUGUI shopItemName;
    [SerializeField] private TextMeshProUGUI shopItemDescription;
    [SerializeField] private Image shopItemIcon;
    [SerializeField] private TextMeshProUGUI shopItemAmountText;
    [SerializeField] private TextMeshProUGUI shopItemPrice;
    [SerializeField] private TextMeshProUGUI buySellButtonText;
    [SerializeField] private Button buySellButton;
    [SerializeField] private TextMeshProUGUI treasureSellValue;
    [SerializeField] private Button treasureSellButton;
    [SerializeField] private Button shopIncreaseAmountButton;
    [SerializeField] private Button shopDecreaseAmountButton;
    [SerializeField] private Button shopMaxAmountButton;
    [SerializeField] private Button shopMinAmountButton;
    [SerializeField] private TextMeshProUGUI playerCoinAmountShop;
    [SerializeField] private Button[] shopInventoryTabs;
    [SerializeField] private int shopInventorySize;
    [SerializeField] private GameObject shopInventoryTabBox;
    [SerializeField] private GameObject shopItemQuantityBox;
    [SerializeField] private GameObject shopEquipmentStatsBox;
    [SerializeField] private GameObject shopEquipmentInventoryHeaderBox;
    [SerializeField] private TextMeshProUGUI shopEquipmentStat1;
    [SerializeField] private TextMeshProUGUI shopEquipmentStat2;
    private InventorySlot CurrentShopSlot;
    private List<InventorySlot> shopSlotList = new List<InventorySlot>(); 
    private int currentShopInventory = 0;
    
    [Header("Skills - Attributes")]
    [SerializeField] private Image[] skillsCharacterIcons;
    [SerializeField] private GameObject[] skillsQuestionMarks;
    [SerializeField] private GameObject[] skillsPartyMemberShrouds;
    [SerializeField] private TextMeshProUGUI[] skillsNames;
    [SerializeField] private TextMeshProUGUI[] skillsAvailablePoints;
    [SerializeField] private TextMeshProUGUI[] skillsHealth;
    [SerializeField] private TextMeshProUGUI[] skillsDefence;
    [SerializeField] private TextMeshProUGUI[] skillsAttack;
    [SerializeField] private TextMeshProUGUI[] skillsCritChance;
    [SerializeField] private TextMeshProUGUI[] skillsMana;
    [SerializeField] private TextMeshProUGUI[] skillsHealthIncrease;
    [SerializeField] private TextMeshProUGUI[] skillsDefenceIncrease;
    [SerializeField] private TextMeshProUGUI[] skillsAttackIncrease;
    [SerializeField] private TextMeshProUGUI[] skillsCritChanceIncrease;
    [SerializeField] private TextMeshProUGUI[] skillsManaIncrease;
    [SerializeField] private TextMeshProUGUI[] skillsHealthIncreaseAmount;
    [SerializeField] private TextMeshProUGUI[] skillsDefenceIncreaseAmount;
    [SerializeField] private TextMeshProUGUI[] skillsAttackIncreaseAmount;
    [SerializeField] private TextMeshProUGUI[] skillsCritChanceIncreaseAmount;
    [SerializeField] private TextMeshProUGUI[] skillsManaIncreaseAmount;
    [SerializeField] private Button[] skillsHealthIncreaseButtons;
    [SerializeField] private Button[] skillsHealthDecreaseButtons;
    [SerializeField] private Button[] skillsDefenceIncreaseButtons;
    [SerializeField] private Button[] skillsDefenceDecreaseButtons;
    [SerializeField] private Button[] skillsAttackIncreaseButtons;
    [SerializeField] private Button[] skillsAttackDecreaseButtons;
    [SerializeField] private Button[] skillsCritChanceIncreaseButtons;
    [SerializeField] private Button[] skillsCritChanceDecreaseButtons;
    [SerializeField] private Button[] skillsManaIncreaseButtons;
    [SerializeField] private Button[] skillsManaDecreaseButtons;
    [SerializeField] private Button[] applySkillPointsButtons;
    [SerializeField] private TextMeshProUGUI skillsCurrentLevelText;
    [SerializeField] private TextMeshProUGUI skillsNextLevelText;
    [SerializeField] private RectTransform skillsExpBar;
    
    [Header("Skills - Skill Tree")]
    [SerializeField] private GameObject skillTreeScreen;
    [SerializeField] private GameObject[] skillTreeButtonShrouds;
    [SerializeField] private GameObject[] skillSelectionBorders;
    [SerializeField] private Image selectedSkillImage;
    [SerializeField] private TextMeshProUGUI selectedSkillName;
    [SerializeField] private TextMeshProUGUI selectedSkillDescription;
    [SerializeField] private TextMeshProUGUI selectedSkillRequirements;
    [SerializeField] private TextMeshProUGUI selectedSkillUnlockCost;
    [SerializeField] private TextMeshProUGUI[] skillLevelRequirements;
    [SerializeField] private Color skillLockedLevelRequirementColor;
    [SerializeField] private Color skillUnlockedLevelRequirementColor;
    [SerializeField] private Button unlockSelectedSkillButton;
    [SerializeField] private TextMeshProUGUI skillTreeOrbsAmount;
    [SerializeField] private TextMeshProUGUI skillTreeCardOrbsAmount;
    [SerializeField] private int levelRequirementIntervals;
    [SerializeField] private Color skillTreeSelectedSkillBorderColour;
    [SerializeField] private Color skillTreeUnlockedSkillBorderColour;
    [SerializeField] private Image[] skillTreeLinkViewEnemyWeaknesses;
    [SerializeField] private Image[] skillTreeLinkViewEnemyResistances;
    [SerializeField] private Image[] skillTreeLinkIncreaseUltimateCharge2;
    [SerializeField] private Image[] skillTreeLinkIncreaseUltimateCharge3;
    [SerializeField] private Image[] skillTreeLinkUltimateChargeSpeed;
    [SerializeField] private Image[] skillTreeLinkTimingWindow;
    [SerializeField] private Color skillTreeLinkCompleteColour;
    [SerializeField] private Color skillTreeLinkNotCompleteColour;
    private Skill selectedSkill;
        
    [Header("Combat")]
    [SerializeField] private GameObject combatScreen;
    [SerializeField] private GameObject[] combatEnemyLocations;
    [SerializeField] private GameObject[] EnemyTurnArrows;
    [SerializeField] private GameObject[] EnemyTargetPartyMemberArrows;
    [SerializeField] private Image[] combatEnemyImages;
    [SerializeField] private GameObject[] combatEnemySelections;
    [SerializeField] private GameObject[] combatPartyMemberLocations;
    [SerializeField] private Image[] combatPartyMemberImages;
    [SerializeField] private GameObject[] combatPartyMemberSelections;
    [SerializeField] private GameObject[] combatNavigationQE;
    [SerializeField] private GameObject[] combatNavigationAD;
    [SerializeField] private Image combatUltimateChargeWheel;
    [SerializeField] private GameObject[] combatUltimateCharges;
    [SerializeField] private GameObject[] combatUltimateChargeFills;
    [SerializeField] private Button ultimateAttackButton;
    [SerializeField] private Image combatEnemyInfoIcon;
    [SerializeField] private TextMeshProUGUI combatEnemyInfoName;
    [SerializeField] private TextMeshProUGUI combatEnemyInfoHealthAmount;
    [SerializeField] private RectTransform combatEnemyInfoHealthBar;
    [SerializeField] private Image[] combatEnemyInfoWeaknesses;
    [SerializeField] private TextMeshProUGUI combatEnemyInfoUnknownWeakness;
    [SerializeField] private Image[] combatEnemyInfoResistances;
    [SerializeField] private TextMeshProUGUI combatEnemyInfoUnknownResistance;
    [SerializeField] private Button combatInventoryButton;
    [SerializeField] private Button combatAttackMovesButton;
    [SerializeField] private Color selectedActionButtonColor;
    [SerializeField] private Color notSelectedActionButtonColor;
    [SerializeField] private TextMeshProUGUI combatSelectedPlayerName;
    [SerializeField] private Transform combatActionsList;
    [SerializeField] private GameObject normalAttackPrefab;
    [SerializeField] private GameObject skillAttackPrefab;
    [SerializeField] private GameObject combatItemPrefab;
    [SerializeField] private GameObject combatSelectedItemInfo;
    [SerializeField] private GameObject combatSelectedAttackInfo;
    [SerializeField] private GameObject combatNoSelectedActionInfo;
    [SerializeField] private TextMeshProUGUI combatItemName;
    [SerializeField] private TextMeshProUGUI combatItemDescription;
    [SerializeField] private Image combatItemIcon;
    [SerializeField] private TextMeshProUGUI selectedAttackName;
    [SerializeField] private TextMeshProUGUI selectedAttackDescription;
    [SerializeField] private TextMeshProUGUI selectedAttackMPCost;
    [SerializeField] private GameObject selectedAttackDamageTypeBox;
    [SerializeField] private Image selectedAttackDamageTypeIcon;
    [SerializeField] private Sprite deadEnemySprite;
    [SerializeField] private GameObject playerActionsBlocker;
    private int combatSelectedEnemyIndex;
    private int combatSelectedPartyMemberIndex;
    private int combatEnemyTargetPartyMemberIndex;
    private int unlockedPartyMembers;
    private bool isAttackListOpen;
    
    [Header("Combat - Selection")]
    [SerializeField] private Material combatSelectionMaterial;
    [SerializeField] private GameObject combatSelectionScreen;
    [SerializeField] private RectTransform combatSelectionScreenBackground;
    [SerializeField] private float combatSelectionHoleRadius = 0.08f;
    [SerializeField] private GameObject[] combatSelectionRings;
    [SerializeField] private GameObject[] combatSelectionButtons;
    [SerializeField] private GameObject ultimateAttackCostBox;
    [SerializeField] private GameObject[] ultimateAttackCost;
    [SerializeField] private Button castUltimateButton;
    [SerializeField] private TextMeshProUGUI teamUpBonusText;
    [SerializeField] private Transform ultimateAttackEffectsList;
    [SerializeField] private GameObject ultimateAttackDamageTypePrefab;
    [SerializeField] private GameObject useItemSelectionButton;
    [SerializeField] private GameObject useItemPartyMemberInfoBox;
    [SerializeField] private GameObject[] useItemPartyMemberInfo;
    [SerializeField] private Image[] useItemPartyMemberInfoImages;
    [SerializeField] private TextMeshProUGUI[] useItemPartyMemberInfoNames;
    [SerializeField] private TextMeshProUGUI[] useItemPartyMemberInfoHealthAmount;
    [SerializeField] private TextMeshProUGUI[] useItemPartyMemberInfoHealthRecoveryAmount;
    [SerializeField] private TextMeshProUGUI[] useItemPartyMemberInfoManaAmount;
    [SerializeField] private TextMeshProUGUI[] useItemPartyMemberInfoManaRecoveryAmount;
    [SerializeField] private RectTransform[] useItemPartyMemberInfoHealthBars;
    [SerializeField] private RectTransform[] useItemPartyMemberInfoHealthRecoveryBars;
    [SerializeField] private RectTransform[] useItemPartyMemberInfoManaBars;
    [SerializeField] private RectTransform[] useItemPartyMemberInfoManaRecoveryBars;
    [SerializeField] private GameObject useItemInfo;
    [SerializeField] private Image useItemIcon;
    [SerializeField] private TextMeshProUGUI useItemName;
    [SerializeField] private TextMeshProUGUI useItemDescription;
    [SerializeField] private Image[] useItemPartyMemberIcons;
    [HideInInspector] public ItemConsumable selectedCombatItem;
    [HideInInspector] public bool[] combatSelections;
    [HideInInspector] public int numberOfSelectedPartyMembers;
    private int maxNumberOfCombatSelections;
    private bool isUltimateAttackSelection;
    
    [Header("Party Info")] 
    [SerializeField] private GameObject partyMemberInfoBox;
    [SerializeField] private GameObject[] partyMemberInfo;
    [SerializeField] private Image[] partyMemberInfoImages;
    [SerializeField] private TextMeshProUGUI[] partyMemberInfoNames;
    [SerializeField] private TextMeshProUGUI[] partyMemberInfoHealthAmount;
    [SerializeField] private TextMeshProUGUI[] partyMemberInfoManaAmount;
    [SerializeField] private RectTransform[] partyMemberInfoHealthBars;
    [SerializeField] private RectTransform[] partyMemberInfoManaBars;
    
    [Header("Damage Text")]
    [SerializeField] private GameObject combatTextPrefab;
    [SerializeField] private Transform combatTextsParent;
    [SerializeField] private int combatTextPoolSize = 30;
    private int combatTextNextPoolNumber;
    private CombatText[] combatTextPool;
    
    
    private PlayerActions actions;
    
    // Managers
    private SkillsManager skillsManager;
    private CombatManager combatManager;
    private EquipmentManager equipmentManager;
    private Inventory inventory;
    
    #endregion

    #region Awake, Start, Update
    
    protected override void Awake()
    {
        base.Awake();
        actions = new PlayerActions();
        
        sliderRect = timingSlider.GetComponent<RectTransform>();
        barRect = timingBar.GetComponent<RectTransform>();
        windowRect = timingWindow.GetComponent<RectTransform>();
    }

    private void Start()
    {
        skillsManager = SkillsManager.Instance;
        combatManager = CombatManager.Instance;
        equipmentManager = EquipmentManager.Instance;
        inventory = Inventory.Instance;
        
        actions.General.Respawn.performed += ctx => SetIsReviving(true);  
        actions.General.Respawn.canceled += ctx => SetIsReviving(false); 
        actions.General.TabMenu.performed += ctx => OpenCloseTabMenu();
        actions.General.Pause.performed += ctx => CloseMenu();
        
        actions.UI.LeftTab.performed += ctx => SwitchTab(-1);
        actions.UI.RightTab.performed += ctx => SwitchTab(1);
        actions.UI.LeftInv.performed += ctx => SwitchInventory(-1);
        actions.UI.RightInv.performed += ctx => SwitchInventory(1);

        actions.Combat.SelectEnemyLeft.performed += ctx => SwitchSelectedEnemy(-1);
        actions.Combat.SelectEnemyRight.performed += ctx => SwitchSelectedEnemy(1);
        actions.Combat.SelectPartyMemberLeft.performed += ctx => SwitchSelectedPartyMember(-1);
        actions.Combat.SelectPartyMemberRight.performed += ctx => SwitchSelectedPartyMember(1);
        actions.Combat.UltimateAttack.performed += ctx => CombatSelection(true);
        actions.Combat.TimingWindow.performed += ctx => StopTimingSlider();
        
        combatSelections = new bool[combatSelectionRings.Length];
        
        InitialiseInventory();
        InitialiseEquipmentInventory();
        InitialiseShopInventories();
        InstantiateCombatTextPool();
    }
    
    #endregion

    #region PartyInfo
    public void UpdatePartyMemberInfo()
    {
        for (int i = 0; i < skillsManager.partyMembers.Length; i++)
        {
            if(skillsManager.partyMembers[i].IsUnlocked) FillPartyMemberInfo(i);
            else partyMemberInfo[i].gameObject.SetActive(false);
        }
    }

    private void FillPartyMemberInfo(int index)
    {
        partyMemberInfo[index].gameObject.SetActive(true);
        partyMemberInfoNames[index].text = skillsManager.partyMembers[index].Name;
        partyMemberInfoImages[index].sprite = skillsManager.partyMembers[index].IconFront;

        partyMemberInfoHealthAmount[index].text = combatManager.GetPartyMemberCurrentHealth(index).ToString();
        partyMemberInfoManaAmount[index].text = combatManager.GetPartyMemberCurrentMana(index).ToString();

        Vector3 scale = partyMemberInfoHealthBars[index].localScale;
        scale.x = combatManager.GetPartyMemberCurrentHealthPercentage(index);
        partyMemberInfoHealthBars[index].localScale = scale;
        scale.x = combatManager.GetPartyMemberCurrentManaPercentage(index);
        partyMemberInfoManaBars[index].localScale = scale;
    }

    private void ShowPartyMemberInfo()
    {
        partyMemberInfoBox.SetActive(true);
        UpdatePartyMemberInfo();
    }

    private void HidePartyMemberInfo()
    {
        partyMemberInfoBox.SetActive(false);
    }

    #endregion
    
    #region Combat - Timing Window
    
    private void StartTiming(bool isAttack, float speed)
    {
        isTiming = true;
        isAttackTiming = isAttack;
        
        float width = timingWindowBaseSize;

        if (isAttack && skillsManager.GetSkill(SkillTreeSkills.IncreaseAttackTimingWindow).IsUnlocked)
        {
            width *= skillsManager.timingWindowIncreaseMultiplier;
        }

        if (!isAttack && skillsManager.GetSkill(SkillTreeSkills.IncreaseBlockTimingWindow).IsUnlocked)
        {
            width *= skillsManager.timingWindowIncreaseMultiplier;
        }

        windowRect.sizeDelta = new Vector2(width, windowRect.sizeDelta.y);

        StartCoroutine(MoveTimingSlider(speed));
    }

    private IEnumerator MoveTimingSlider(float speed)
    {
        float barWidth = barRect.rect.width;

        float posX = -barWidth / 2f; 

        sliderRect.anchoredPosition = new Vector2(posX, sliderRect.anchoredPosition.y);
        
        while (isTiming)
        {
            posX += speed * Time.deltaTime;

            sliderRect.anchoredPosition = new Vector2(posX, sliderRect.anchoredPosition.y);
            
            yield return null;
            
            if (posX >= barWidth / 2f)
            {
                isTiming = false;
                combatManager.isTiming = false;
                combatManager.perfectTimed = false;
            }
        }
        
        yield return new WaitForSeconds(1f);
        
        HideTimingWindow();
    }

    private void StopTimingSlider()
    {
        if(!isTiming) return;
        
        isTiming = false;
        combatManager.isTiming = false;
        
        float sliderX = sliderRect.anchoredPosition.x;

        float windowCenter = windowRect.anchoredPosition.x;
        float windowHalfWidth = windowRect.rect.width / 2f;

        bool perfectTiming = Mathf.Abs(sliderX - windowCenter) <= windowHalfWidth;
        combatManager.perfectTimed = perfectTiming;

        if (perfectTiming)
        {
            if (isAttackTiming)
            {
                combatPartyMemberImages[combatSelectedPartyMemberIndex].sprite = skillsManager.partyMembers[combatSelectedPartyMemberIndex].IconPerfectTiming;
            }
            else
            {
                combatPartyMemberImages[combatEnemyTargetPartyMemberIndex].sprite = skillsManager.partyMembers[combatEnemyTargetPartyMemberIndex].IconPerfectTiming;
            }
            combatManager.GetPerfectTimingCharge();
        }
    }

    public void ShowTimingWindow(bool isAttack, float speed)
    {
        timingWindowScreen.SetActive(true);
        combatManager.isTiming = true;
        isTiming = true;
        StartTiming(isAttack, speed);
    }

    private void HideTimingWindow()
    {
        timingWindowScreen.SetActive(false);
        isTiming = false;
        combatManager.isTiming = false;
        for (int i = 0; i < combatPartyMemberImages.Length; i++)
        {
            combatPartyMemberImages[i].sprite = skillsManager.partyMembers[i].IconBack;
        }
    }

    #endregion

    #region Combat - DamageText

    private void InstantiateCombatTextPool()
    {
        combatTextPool = new CombatText[combatTextPoolSize];

        for (int i = 0; i < combatTextPoolSize; i++)
        {
            combatTextPool[i] = Instantiate(combatTextPrefab, combatTextsParent).GetComponent<CombatText>();
            combatTextPool[i].DisableText();
        }
        
        combatTextNextPoolNumber = 0;
    }

    public void ShowEnemyCombatText(int enemyIndex, int damageAmount, CombatTextType type)
    {
        RectTransform enemy = combatEnemyLocations[enemyIndex].GetComponent<RectTransform>();
        ShowCombatText(damageAmount, enemy, type);
    }

    public void ShowPartyMemberCombatText(int partyMemberIndex, int damageAmount, CombatTextType type)
    {
        RectTransform partymember = combatPartyMemberLocations[partyMemberIndex].GetComponent<RectTransform>();
        ShowCombatText(damageAmount, partymember, type);
    }
    
    private void ShowCombatText(int damageAmount, RectTransform parent, CombatTextType type)
    {
        Vector3 topRightLocal = new Vector3(parent.rect.width / 2f, parent.rect.height / 2f, 0f);
        Vector3 topRightWorld = parent.TransformPoint(topRightLocal);
        
        CombatText text = combatTextPool[combatTextNextPoolNumber];
        text.gameObject.SetActive(true);
        text.transform.position = topRightWorld;
        text.SetDamageText(damageAmount, type);
        
        combatTextNextPoolNumber++;
        if(combatTextNextPoolNumber >= combatTextPool.Length) combatTextNextPoolNumber = 0;
    }

    private void DisableAllCombatText()
    {
        foreach (CombatText text in combatTextPool)
        {
            text.DisableText();
        }
    }

    #endregion

    #region Combat - Selection Screen
    
    private void SelectCombatItem(ItemConsumable item)
    {
        selectedCombatItem = item;
        CombatSelection(false);
        MakeCombatSelection(combatSelectedPartyMemberIndex);
    }

    public void CombatSelection(bool isUltimate)
    {
        isUltimateAttackSelection = isUltimate;
        EnterCombatSelectionScreen();
    }

    public void MakeCombatSelection(int index)
    {
        if (isUltimateAttackSelection)
        {
            if (!combatManager.IsPartyMemberDead(index))
            {
                bool isSelected = combatSelections[index];

                if (isSelected)
                {
                    combatSelections[index] = false;
                    numberOfSelectedPartyMembers--;
                    combatSelectionRings[index].SetActive(false);
                    combatPartyMemberImages[index].sprite = skillsManager.partyMembers[index].IconBack;
                }
                else if (numberOfSelectedPartyMembers < maxNumberOfCombatSelections)
                {
                    combatSelections[index] = true;
                    numberOfSelectedPartyMembers++;
                    combatSelectionRings[index].SetActive(true);
                    combatPartyMemberImages[index].sprite = skillsManager.partyMembers[index].IconUltimate;
                }
            }

            UpdateCombatUltimateSelectionInfo();
        }
        else
        {
            if (selectedCombatItem.IsWholeParty)
            {
                
                for (int i = 0; i < combatSelections.Length; i++)
                {
                    bool isSelected = skillsManager.partyMembers[i].IsUnlocked;
                    if (selectedCombatItem.IsRevive && isSelected)
                    {
                        if (!combatManager.IsPartyMemberDead(i)) isSelected = false;
                    } 
                    combatSelections[i] = isSelected;
                    combatSelectionRings[i].SetActive(isSelected);

                    if (isSelected)
                    {
                        combatPartyMemberImages[i].sprite = skillsManager.partyMembers[i].IconItem;
                        useItemPartyMemberIcons[i].gameObject.SetActive(true);
                        useItemPartyMemberIcons[i].sprite = selectedCombatItem.Icon;
                    }
                }
            }
            else
            {
                for (int i = 0; i < combatSelections.Length; i++)
                {
                    combatSelections[i] = false;
                    combatSelectionRings[i].SetActive(false);
                    combatPartyMemberImages[i].sprite = skillsManager.partyMembers[i].IconBack;
                    useItemPartyMemberIcons[i].gameObject.SetActive(false);
                }

                combatSelections[index] = true;
                combatSelectionRings[index].SetActive(true);
                combatPartyMemberImages[index].sprite = skillsManager.partyMembers[index].IconItem;
                useItemPartyMemberIcons[index].gameObject.SetActive(true);
                useItemPartyMemberIcons[index].sprite = selectedCombatItem.Icon;
            }

            UpdateUseItemSelectionInfo(index);
        }
    }
    
    private void UpdateUseItemSelectionInfo(int selectedIndex)
    {
        ClearCombatSelectionScreenInfo();
        
        useItemSelectionButton.SetActive(true);
        useItemPartyMemberInfoBox.SetActive(true);
        useItemInfo.SetActive(true);

        bool isUsable = false;
        for(int i = 0; i< combatSelections.Length; i++)
        {
            if(combatSelections[i]) isUsable = true;

            if (selectedCombatItem.IsRevive && combatSelections[i] && !combatManager.IsPartyMemberDead(i))
            {
                isUsable = false;
                break;
            }
        }
        useItemSelectionButton.GetComponent<Button>().interactable = isUsable;
        
        useItemName.text = selectedCombatItem.Name;
        useItemIcon.sprite = selectedCombatItem.Icon;
        useItemDescription.text = selectedCombatItem.Description;
        
        foreach (GameObject info in useItemPartyMemberInfo)
        {
            info.SetActive(false);
        }

        if (selectedCombatItem.IsWholeParty)
        {
            for (int i = 0; i < useItemPartyMemberInfo.Length; i++)
            {
                if(combatSelections[i]) FillUseItemPartyInfo(i);
            }
        }
        else
        {
            FillUseItemPartyInfo(selectedIndex);
        }
    }

    private void FillUseItemPartyInfo(int index)
    {
        useItemPartyMemberInfo[index].gameObject.SetActive(true);
        useItemPartyMemberInfoNames[index].text = skillsManager.partyMembers[index].Name;
        useItemPartyMemberInfoImages[index].sprite = skillsManager.partyMembers[index].IconFront;

        useItemPartyMemberInfoHealthAmount[index].text = combatManager.GetPartyMemberCurrentHealth(index).ToString();
        useItemPartyMemberInfoManaAmount[index].text = combatManager.GetPartyMemberCurrentMana(index).ToString();

        Vector3 scale = useItemPartyMemberInfoHealthBars[index].localScale;
        scale.x = combatManager.GetPartyMemberCurrentHealthPercentage(index);
        useItemPartyMemberInfoHealthBars[index].localScale = scale;
        scale.x = combatManager.GetPartyMemberCurrentManaPercentage(index);
        useItemPartyMemberInfoManaBars[index].localScale = scale;
        
        scale.x = combatManager.GetPartyMemberHealthRecoveryPercentage(index, selectedCombatItem);
        useItemPartyMemberInfoHealthRecoveryBars[index].localScale = scale;
        if (selectedCombatItem.GetHealthValue() > 0)
        {
            if (scale.x >= 1)
            {
                int healthDifference = skillsManager.partyMembers[index].CurrentMaxHealth - combatManager.GetPartyMemberCurrentHealth(index);
                useItemPartyMemberInfoHealthRecoveryAmount[index].text = "+" + healthDifference;
            }
            else useItemPartyMemberInfoHealthRecoveryAmount[index].text = "+" + selectedCombatItem.GetHealthValue();
        }
        else useItemPartyMemberInfoHealthRecoveryAmount[index].text = "";
        
        scale.x = combatManager.GetPartyMemberManaRecoveryPercentage(index, selectedCombatItem);
        useItemPartyMemberInfoManaRecoveryBars[index].localScale = scale;
        if (selectedCombatItem.GetManaValue() > 0)
        {
            if (scale.x >= 1)
            {
                int manaDifference = skillsManager.partyMembers[index].CurrentMaxMana - combatManager.GetPartyMemberCurrentMana(index);
                useItemPartyMemberInfoManaRecoveryAmount[index].text = "+" + manaDifference;
            }
            else useItemPartyMemberInfoManaRecoveryAmount[index].text = "+" +  selectedCombatItem.GetManaValue();
        }
        else useItemPartyMemberInfoManaRecoveryAmount[index].text = "";
    }

    private void ClearCombatSelectionScreenInfo()
    {
        castUltimateButton.gameObject.SetActive(false);
        teamUpBonusText.gameObject.SetActive(false);
        ClearChildren(ultimateAttackEffectsList);
        ultimateAttackCostBox.gameObject.SetActive(false);
        useItemSelectionButton.SetActive(false);
        useItemPartyMemberInfoBox.SetActive(false);
        useItemInfo.SetActive(false);
    }

    private void UpdateCombatUltimateSelectionInfo()
    {
        ClearCombatSelectionScreenInfo();
        
        castUltimateButton.gameObject.SetActive(true);
        castUltimateButton.interactable = numberOfSelectedPartyMembers > 0;
        SetUltimateAttackCost();

        string teamUpText = "+ Team Up Bonus ";
        if (numberOfSelectedPartyMembers > 1)
        {
            teamUpText += "I";
            teamUpBonusText.gameObject.SetActive(true);
        }
        if (numberOfSelectedPartyMembers > 2) teamUpText += "I";
        teamUpBonusText.text = teamUpText;
        
        for (int i = 0; i < combatSelections.Length; i++)
        {
            if (combatSelections[i])
            {
                List<DamageType> damageTypes = equipmentManager.GetPartyMemberDamageTypes(i);

                foreach (DamageType damageType in damageTypes)
                {
                    if(damageType.damageType == DamageTypes.None) continue;
                    GameObject attackEffect;
                    attackEffect = Instantiate(ultimateAttackDamageTypePrefab, ultimateAttackEffectsList);
                    attackEffect.GetComponent<Image>().sprite = damageType.icon;
                }
            }
        }
    }

    private void SetUltimateAttackCost()
    {
        ultimateAttackCostBox.gameObject.SetActive(true);
        for (int i = 0; i < ultimateAttackCost.Length; i++)
        {
            ultimateAttackCost[i].SetActive(combatSelections[i]);
        }
    }
    
    private void EnterCombatSelectionScreen()
    {
        combatSelectionScreen.SetActive(true);
        for (int i = 0; i < combatSelections.Length; i++)
        {
            combatSelections[i] = false;
            combatSelectionRings[i].SetActive(false);
        }
        numberOfSelectedPartyMembers = 0;
        maxNumberOfCombatSelections = combatManager.ultimateCharges;
        
        if(isUltimateAttackSelection) UpdateCombatUltimateSelectionInfo();
    }

    public void ExitCombatSelectionScreen()
    {
        combatSelectionScreen.SetActive(false);
        for (int i = 0; i < combatPartyMemberImages.Length; i++)
        {
            combatPartyMemberImages[i].sprite = skillsManager.partyMembers[i].IconBack;
            useItemPartyMemberIcons[i].gameObject.SetActive(false);
        }
        HideCombatActionInfo();
    }
    
    private void SetCombatSelectionHoles()
    {
        for (int i = 0; i < skillsManager.partyMembers.Length; i++)
        {
            if (skillsManager.partyMembers[i].IsUnlocked)
            {
                SetCombatSelectionHole(i, combatPartyMemberLocations[i].GetComponent<RectTransform>());
                combatSelectionButtons[i].SetActive(true);
            }
            else
            {
                combatSelectionMaterial.SetVector($"_Hole{i}Pos", new Vector2(-1000, -1000));
                combatSelectionButtons[i].SetActive(false);
            }
        }
        
        float width = combatSelectionScreenBackground.rect.width;
        float height = combatSelectionScreenBackground.rect.height;
        Vector2 overlayScaleFactor = new Vector2(width/height, 1f);

        combatSelectionMaterial.SetVector("_OverlayScale", overlayScaleFactor);
        combatSelectionMaterial.SetFloat("_HoleRadius", combatSelectionHoleRadius);
    }
    
    private void SetCombatSelectionHole(int index, RectTransform targetRect)
    {
        // Convert UI element position to overlay local space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            combatSelectionScreenBackground,
            RectTransformUtility.WorldToScreenPoint(null, targetRect.position),
            null, 
            out Vector2 localPos
        );

        // Convert local position to UV (0-1)
        Vector2 uvPos = new Vector2(
            (localPos.x / combatSelectionScreenBackground.rect.width) + 0.5f,
            (localPos.y / combatSelectionScreenBackground.rect.height) + 0.5f
        );
        
        combatSelectionMaterial.SetVector($"_Hole{index}Pos", uvPos);
    }

    #endregion

    #region Combat

    public void ActivateCombatScreen(List<EnemyDetails> enemies)
    {
        combatScreen.SetActive(true);
        actions.Combat.Enable();
        ShowPartyMemberInfo();

        foreach (GameObject location in combatEnemyLocations)
        {
            location.SetActive(false);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            combatEnemyLocations[i].SetActive(true);
            combatEnemyImages[i].sprite = enemies[i].EnemySprite;
        }
        foreach (GameObject navigation in combatNavigationQE)
        {
            navigation.SetActive(enemies.Count > 1);
        }

        unlockedPartyMembers = 0;
        for (int i = 0; i < skillsManager.partyMembers.Length; i++)
        {
            if (skillsManager.partyMembers[i].IsUnlocked)
            {
                unlockedPartyMembers++;
                combatPartyMemberLocations[i].SetActive(true);
                combatPartyMemberImages[i].sprite = skillsManager.partyMembers[i].IconBack;
            }
            else
            {
                combatPartyMemberLocations[i].SetActive(false);
            }
        }
        
        foreach (GameObject navigation in combatNavigationAD)
        {
            navigation.SetActive(unlockedPartyMembers > 1);
        }
        
        SelectEnemy(0);
        SelectCombatPartyMember(0);
        UpdateUltimateCharges();
        Canvas.ForceUpdateCanvases();
        SetCombatSelectionHoles();
        HideCombatActionInfo();
        OpenAttackMovesList();
        ExitCombatSelectionScreen();
    }

    public void ShowEnemyTurnArrow(int index)
    {
        ClearEnemyTurnArrows();
        EnemyTurnArrows[index].SetActive(true);
    }

    public void ClearEnemyTurnArrows()
    {
        foreach (GameObject arrow in EnemyTurnArrows)
        {
            arrow.SetActive(false);
        }
    }

    public void SetEnemyTarget(int index)
    {
        ClearEnemyTargetArrows();
        combatEnemyTargetPartyMemberIndex = index;
        EnemyTargetPartyMemberArrows[index].SetActive(true);
    }

    public void ClearEnemyTargetArrows()
    {
        foreach (GameObject arrow in EnemyTargetPartyMemberArrows)
        {
            arrow.SetActive(false);
        }
    }
    
    public void RevivePartyMember(int partyMemberIndex)
    {
        combatPartyMemberImages[partyMemberIndex].sprite = skillsManager.partyMembers[partyMemberIndex].IconBack;
    }

    public void KillPartyMember(int partyMemberIndex)
    {
        combatPartyMemberImages[partyMemberIndex].sprite = skillsManager.partyMembers[partyMemberIndex].IconDead;
    }
    
    public void StartPlayersTurn()
    {
        playerActionsBlocker.SetActive(false);
        RefreshPlayerActions();
    }

    public void StartEnemyTurn()
    {
        playerActionsBlocker.SetActive(true);
    }
    
    public void KillEnemy(int enemy)
    {
        combatEnemyImages[enemy].sprite = deadEnemySprite;
    }
    
    private void SelectAttackMove(AttackMove attack)
    {
        playerActionsBlocker.SetActive(true);
        if (attack.IsHeal)
        {
            combatManager.HealParty(attack);
            return;
        }
        combatManager.AttackEnemy(attack);
    }

    public void OpenCombatInventory()
    {
        FillCombatItems();
        
        ColorBlock invColors = combatInventoryButton.colors;
        invColors.normalColor = selectedActionButtonColor;
        combatInventoryButton.colors = invColors;
        
        ColorBlock attColors = combatAttackMovesButton.colors;
        attColors.normalColor = notSelectedActionButtonColor;
        combatAttackMovesButton.colors = attColors;
        
        isAttackListOpen = false;
    }

    public void OpenAttackMovesList()
    {
        FillAttackMoves();
        
        ColorBlock invColors = combatInventoryButton.colors;
        invColors.normalColor = notSelectedActionButtonColor;
        combatInventoryButton.colors = invColors;
        
        ColorBlock attColors = combatAttackMovesButton.colors;
        attColors.normalColor = selectedActionButtonColor;
        combatAttackMovesButton.colors = attColors;

        isAttackListOpen = true;
    }

    private void FillCombatItems()
    {
        ClearChildren(combatActionsList);
        ItemConsumable[] consumables = inventory.InventoryItemsConsumables;
        
        foreach (ItemConsumable item in consumables)
        {
            if(item == null) continue;
            
            int amount = combatManager.GetItemAmountLeft(item.ID);

            if (amount > 0)
            {
                GameObject combatItem = Instantiate(combatItemPrefab, combatActionsList);
                combatItem.GetComponent<CombatItemButton>().FillDetails(item, amount);
                
                ItemConsumable itemCopy = item;
                combatItem.GetComponent<Button>().onClick.AddListener(() => SelectCombatItem(itemCopy));
            }
        }
    }

    private void FillAttackMoves()
    {
        ClearChildren(combatActionsList);
        AttackMove[] attacks = combatManager.GetAllPartyMemberAttacks(combatSelectedPartyMemberIndex);

        foreach (AttackMove attack in attacks)
        {
            GameObject attackMoveButton;
            attackMoveButton = Instantiate(attack.Type == AttackType.Basic ? normalAttackPrefab : skillAttackPrefab, combatActionsList);

            attackMoveButton.GetComponent<AttackMoveButton>().Instantiate(attack);
            Button button = attackMoveButton.GetComponent<Button>();
            button.onClick.AddListener(() => SelectAttackMove(attack));
            
            if(attack.Type == AttackType.Skill && combatManager.GetPartyMemberCurrentMana(combatSelectedPartyMemberIndex) < attack.MPCost) button.interactable = false;
            attackMoveButton.GetComponentInChildren<TextMeshProUGUI>().text = attack.MoveName;
        }
    }
    
    public void ShowCombatMoveInfo(AttackMove attackMove)
    {
        selectedAttackName.text = attackMove.MoveName;
        selectedAttackDescription.text = attackMove.Description;

        selectedAttackMPCost.gameObject.SetActive(false);
        if (attackMove.Type == AttackType.Skill)
        {
            selectedAttackMPCost.gameObject.SetActive(true);
            selectedAttackMPCost.text = attackMove.MPCost + "MP";
        }
        
        DamageType damageType = attackMove.DamageType;
        if (damageType.damageType == DamageTypes.None)
        {
            selectedAttackDamageTypeBox.gameObject.SetActive(false);
        }
        else
        {
            selectedAttackDamageTypeBox.gameObject.SetActive(true);
            selectedAttackDamageTypeIcon.sprite = damageType.icon;
        }
        
        combatNoSelectedActionInfo.SetActive(false);
        combatSelectedAttackInfo.SetActive(true);
        combatSelectedItemInfo.SetActive(false);
    }

    public void ShowCombatItemInfo(InventoryItem item)
    {
        combatItemName.text = item.Name;
        combatItemDescription.text = item.Description;
        combatItemIcon.gameObject.SetActive(true);
        combatItemIcon.sprite = item.Icon;
        
        combatNoSelectedActionInfo.SetActive(false);
        combatSelectedAttackInfo.SetActive(false);
        combatSelectedItemInfo.SetActive(true);
    }
    
    public void HideCombatActionInfo()
    {
        if (combatSelectionScreen.activeSelf) return;
        
        combatNoSelectedActionInfo.SetActive(true);
        combatSelectedAttackInfo.SetActive(false);
        combatSelectedItemInfo.SetActive(false);
    }
    
    public void SwitchSelectedEnemy(int direction)
    {
        int enemyCount = combatManager.NumberOfEnemies();
        if (enemyCount <= 1) return;

        int attempts = enemyCount;
        
        do
        {
            combatSelectedEnemyIndex += direction;
            if (combatSelectedEnemyIndex < 0) combatSelectedEnemyIndex = combatManager.NumberOfEnemies() - 1;
            else if (combatSelectedEnemyIndex >= combatManager.NumberOfEnemies()) combatSelectedEnemyIndex = 0;
            
            attempts--;
        } 
        while (combatManager.IsEnemyDead(combatSelectedEnemyIndex) && attempts > 0);
        
        SelectEnemy(combatSelectedEnemyIndex);
    }
    
    public void SwitchSelectedPartyMember(int direction)
    {
        if (unlockedPartyMembers <= 1) return;

        do
        {
            combatSelectedPartyMemberIndex += direction;
            if (combatSelectedPartyMemberIndex < 0) combatSelectedPartyMemberIndex = skillsManager.partyMembers.Length - 1;
            else if (combatSelectedPartyMemberIndex >= skillsManager.partyMembers.Length) combatSelectedPartyMemberIndex = 0;
        } 
        while (!skillsManager.partyMembers[combatSelectedPartyMemberIndex].IsUnlocked && 
               !combatManager.IsPartyMemberDead(combatSelectedPartyMemberIndex) &&
               !combatManager.HasTakenTurn(combatSelectedPartyMemberIndex));
        
        SelectCombatPartyMember(combatSelectedPartyMemberIndex);
    }

    public void SelectCombatPartyMember(int index)
    {
        if (combatManager.IsPartyMemberDead(index) || combatManager.HasTakenTurn(index)) return;
        
        combatSelectedPartyMemberIndex = index;
        combatManager.selectedPartyMember = index;
        
        combatSelectedPlayerName.text = skillsManager.partyMembers[index].Name;

        foreach (GameObject selection in combatPartyMemberSelections)
        {
            selection.SetActive(false);
        }
        combatPartyMemberSelections[index].SetActive(true);

        RefreshPlayerActions();
    }

    private void RefreshPlayerActions()
    {
        if (isAttackListOpen) OpenAttackMovesList();
        else OpenCombatInventory();
    }

    public void SelectEnemy(int index)
    {
        if(combatManager.IsEnemyDead(index)) return;
        
        combatSelectedEnemyIndex = index;
        combatManager.selectedEnemy = index;
        EnemyDetails selectedEnemy = combatManager.GetSelectedEnemy();

        combatEnemyInfoIcon.sprite = selectedEnemy.EnemySprite;
        combatEnemyInfoName.text = selectedEnemy.EnemyName;

        UpdateEnemyHealthBar();
        UpdateEnemyWeaknesses();
        UpdateEnemyResistances();

        foreach (GameObject selection in combatEnemySelections)
        {
            selection.SetActive(false);
        }
        combatEnemySelections[index].SetActive(true);
    }
    
    public void UpdateUltimateCharges()
    {
        combatUltimateChargeWheel.fillAmount = combatManager.GetUltimateChargeProgressPercentage();
        
        int maxCharges = combatManager.GetMaxUltimateCharges();
        foreach (GameObject ultimateCharge in combatUltimateCharges)
        {
            ultimateCharge.gameObject.SetActive(false);
        }
        for (int i = 1; i < maxCharges; i++)
        {
            combatUltimateCharges[i - 1].gameObject.SetActive(true);
        }

        int ultimateCharges = combatManager.ultimateCharges;
        foreach (GameObject ultimateCharge in combatUltimateChargeFills)
        {
            ultimateCharge.gameObject.SetActive(false);
        }

        if (maxCharges > 1)
        {
            for (int i = 0; i < ultimateCharges; i++)
            {
                combatUltimateChargeFills[i].gameObject.SetActive(true);
            }
        }

        ultimateAttackButton.interactable = ultimateCharges > 0;
    }
    
    private void UpdateEnemyResistances()
    {
        EnemyDetails selectedEnemy = combatManager.GetSelectedEnemy();

        foreach (Image resistance in combatEnemyInfoResistances)
        {
            resistance.gameObject.SetActive(false);
        }
        
        if (skillsManager.GetSkill(SkillTreeSkills.MakeEnemyResistancesVisible).IsUnlocked)
        {
            if (selectedEnemy.resistances.Length > 0)
            {
                combatEnemyInfoUnknownResistance.gameObject.SetActive(false);
                for (int i = 0; i < selectedEnemy.resistances.Length; i++)
                {
                    combatEnemyInfoResistances[i].gameObject.SetActive(true);
                    combatEnemyInfoResistances[i].sprite = selectedEnemy.resistances[i].icon;
                }
            }
            else
            {
                combatEnemyInfoUnknownResistance.gameObject.SetActive(true);
                combatEnemyInfoUnknownResistance.text = "None";
            }
        }
        else
        {
            combatEnemyInfoUnknownResistance.gameObject.SetActive(true);
            combatEnemyInfoUnknownResistance.text = "?";
        }
    }

    private void UpdateEnemyWeaknesses()
    {
        EnemyDetails selectedEnemy = combatManager.GetSelectedEnemy();

        foreach (Image weakness in combatEnemyInfoWeaknesses)
        {
            weakness.gameObject.SetActive(false);
        }
        
        if (skillsManager.GetSkill(SkillTreeSkills.MakeEnemyWeaknessesVisible).IsUnlocked)
        {
            if (selectedEnemy.weaknesses.Length > 0)
            {
                combatEnemyInfoUnknownWeakness.gameObject.SetActive(false);
                for (int i = 0; i < selectedEnemy.weaknesses.Length; i++)
                {
                    combatEnemyInfoWeaknesses[i].gameObject.SetActive(true);
                    combatEnemyInfoWeaknesses[i].sprite = selectedEnemy.weaknesses[i].icon;
                }
            }
            else
            {
                combatEnemyInfoUnknownWeakness.gameObject.SetActive(true);
                combatEnemyInfoUnknownWeakness.text = "None";
            }
        }
        else
        {
            combatEnemyInfoUnknownWeakness.gameObject.SetActive(true);
            combatEnemyInfoUnknownWeakness.text = "?";
        }
    }

    public void UpdateEnemyHealthBar()
    {
        EnemyDetails selectedEnemy = combatManager.GetSelectedEnemy();

        if (skillsManager.GetSkill(SkillTreeSkills.MakeEnemyHealthVisible).IsUnlocked)
        {
            combatEnemyInfoHealthAmount.text = selectedEnemy.CurrentHealth.ToString();

            Vector3 scale = combatEnemyInfoHealthBar.localScale;
            scale.x = selectedEnemy.GetHealthBarPercentage();
            combatEnemyInfoHealthBar.localScale = scale;
        }
        else
        {
            combatEnemyInfoHealthAmount.text = "???";
            
            Vector3 scale = combatEnemyInfoHealthBar.localScale;
            scale.x = 0f; 
            combatEnemyInfoHealthBar.localScale = scale;
        }
    }

    public void DeactivateCombatScreen()
    {
        combatScreen.SetActive(false);
        actions.Combat.Disable();
        DisableAllCombatText();
    }

    #endregion

    #region Skills - Skill Tree

    public void OpenSkillTreeScreen()
    {
        skillTreeScreen.SetActive(true);
        SelectSkill(selectedSkill);
        SetSkillsAvailable();
        UpdateLevelRequirements();
        UpdateSkillOrbsAmount();
        UpdateSkillLinks();
    }

    private void CloseSkillTreeScreen()
    {
        skillTreeScreen.SetActive(false);
    }
    
    private void SetSkillsAvailable()
    {
        for (int i = 0; i < skillTreeButtonShrouds.Length; i++)
        {
            skillTreeButtonShrouds[i].SetActive(!skillsManager.skills[i].IsAvailable());
        }
    }

    public void SelectSkill(Skill newSelectedSkill)
    {
        this.selectedSkill = newSelectedSkill;
        UpdateSelectedSkill(newSelectedSkill);

        foreach (Skill skill in skillsManager.skills)
        {
            if (skill.IsUnlocked)
            {
                skillSelectionBorders[skillsManager.GetSkillIndex(skill)].SetActive(true);
                skillSelectionBorders[skillsManager.GetSkillIndex(skill)].GetComponent<Image>().color = skillTreeUnlockedSkillBorderColour;
            }
            else
            {
                skillSelectionBorders[skillsManager.GetSkillIndex(skill)].SetActive(false);
            }
        }
        skillSelectionBorders[skillsManager.GetSkillIndex(newSelectedSkill)].SetActive(true);
        skillSelectionBorders[skillsManager.GetSkillIndex(newSelectedSkill)].GetComponent<Image>().color = skillTreeSelectedSkillBorderColour;
    }

    private void UpdateSelectedSkill(Skill skill)
    {
        selectedSkillImage.sprite = skill.SkillIcon;
        selectedSkillName.text = skill.SkillName;
        selectedSkillDescription.text = skill.SkillDescription;

        if (skill.HasRequiredSkill() && !skill.RequiredSkill.IsUnlocked)
        {
            selectedSkillRequirements.text = skill.RequiredSkill.SkillName + " required";
        }
        else if (!skill.IsAvailable())
        {
            selectedSkillRequirements.text = "Level " + skill.LevelRequired + " required";
        }
        else
        {
            selectedSkillRequirements.text = "";
        }

        unlockSelectedSkillButton.interactable = (skill.IsAvailable() && skill.CanAffordSkill());
        if(skill.IsUnlocked) unlockSelectedSkillButton.interactable = false;
        
        selectedSkillUnlockCost.text = skill.OrbCost.ToString();
    }

    public void UnlockSelectedSkill()
    {
        skillsManager.UnlockSkill(selectedSkill);
        SetSkillsAvailable();
        UpdateSelectedSkill(selectedSkill);
        UpdateSkillOrbsAmount();
        UpdateSkillLinks();
    }

    public void UpdateSkillOrbsAmount()
    {
        skillTreeOrbsAmount.text = skillsManager.skillOrbs.ToString();
        skillTreeCardOrbsAmount.text = skillsManager.skillOrbs.ToString();
    }

    private void UpdateLevelRequirements()
    {
        for (int i = 0; i < skillLevelRequirements.Length; i++)
        {
            skillLevelRequirements[i].color = i * levelRequirementIntervals > skillsManager.Level ? skillLockedLevelRequirementColor : skillUnlockedLevelRequirementColor;
        }
    }

    private void UpdateSkillLinks()
    {
        foreach (Skill skill in skillsManager.skills)
        {
            if (skill.HasRequiredSkill())
            {
                Color targetColor = skill.IsUnlocked ? skillTreeLinkCompleteColour : skillTreeLinkNotCompleteColour;
                switch (skill.SkillTreeSkill)
                {
                    case SkillTreeSkills.ExtraUltimateCharge2:
                        foreach (Image link in skillTreeLinkIncreaseUltimateCharge2)
                            link.color = targetColor;
                        break;

                    case SkillTreeSkills.ExtraUltimateCharge3:
                        foreach (Image link in skillTreeLinkIncreaseUltimateCharge3)
                            link.color = targetColor;
                        break;

                    case SkillTreeSkills.IncreaseUltimateChargeSpeed:
                        foreach (Image link in skillTreeLinkUltimateChargeSpeed)
                            link.color = targetColor;
                        break;

                    case SkillTreeSkills.IncreaseBlockTimingWindow:
                        foreach (Image link in skillTreeLinkTimingWindow)
                            link.color = targetColor;
                        break;

                    case SkillTreeSkills.MakeEnemyResistancesVisible:
                        foreach (Image link in skillTreeLinkViewEnemyResistances)
                            link.color = targetColor;
                        break;

                    case SkillTreeSkills.MakeEnemyWeaknessesVisible:
                        foreach (Image link in skillTreeLinkViewEnemyWeaknesses)
                            link.color = targetColor;
                        break;
                }
                
            }
        }
    }

    #endregion

    #region Skills - Attributes

    private void UpdateSkillsUI()
    {
        UpdateSkillCards();
        UpdateLevelBar();
        UpdateSkillOrbsAmount();
    }

    public void UpdateLevelBar()
    {
        skillsCurrentLevelText.text = skillsManager.Level.ToString();
        skillsNextLevelText.text = (skillsManager.Level +1).ToString();
        
        Vector3 scale = skillsExpBar.localScale;
        scale.x = skillsManager.GetExpBarPercentage();
        skillsExpBar.localScale = scale;
    }

    public void UpdateSkillCards()
    {
        PartyMember[] partyMembers = skillsManager.partyMembers;

        for (int i = 0; i < partyMembers.Length; i++)
        {
            partyMembers[i].CalculateBaseStats();
            skillsCharacterIcons[i].sprite = partyMembers[i].IconFront;
            if(partyMembers[i].IsUnlocked){
                skillsNames[i].text = partyMembers[i].Name;
                skillsAvailablePoints[i].text = "Available Points: " + (skillsManager.availableAttributePoints[i] - skillsManager.pendingAttributePoints[i]);
            
                skillsHealth[i].text = "Health: " + partyMembers[i].CurrentBaseMaxHealth;
                skillsDefence[i].text = "Defence: " + partyMembers[i].CurrentBaseDefence;
                skillsAttack[i].text = "Attack: " + partyMembers[i].CurrentBaseAttack;
                skillsCritChance[i].text = "Crit: " + partyMembers[i].CurrentBaseCritChance + "%";
                skillsMana[i].text = "Mana: " + partyMembers[i].CurrentBaseMaxMana;

                skillsHealthIncrease[i].text = (partyMembers[i].SkillPointsHealth + skillsManager.pointsToAddHealth[i]).ToString();    
                skillsDefenceIncrease[i].text = (partyMembers[i].SkillPointsDefence + skillsManager.pointsToAddDefence[i]).ToString();
                skillsAttackIncrease[i].text = (partyMembers[i].SkillPointsAttack + skillsManager.pointsToAddAttack[i]).ToString();
                skillsCritChanceIncrease[i].text = (partyMembers[i].SkillPointsCritChance + skillsManager.pointsToAddCritChance[i]).ToString();
                skillsManaIncrease[i].text = (partyMembers[i].SkillPointsMana + skillsManager.pointsToAddMana[i]).ToString();
            
                skillsHealthIncreaseAmount[i].text = (skillsManager.healthIncreasePerLevel * skillsManager.pointsToAddHealth[i]).ToString();    
                skillsDefenceIncreaseAmount[i].text = (skillsManager.defenceIncreasePerLevel * skillsManager.pointsToAddDefence[i]).ToString();
                skillsAttackIncreaseAmount[i].text = (skillsManager.attackIncreasePerLevel * skillsManager.pointsToAddAttack[i]).ToString();
                skillsCritChanceIncreaseAmount[i].text = (skillsManager.critIncreasePerLevel * skillsManager.pointsToAddCritChance[i]).ToString();
                skillsManaIncreaseAmount[i].text = (skillsManager.manaIncreasePerLevel * skillsManager.pointsToAddMana[i]).ToString();
                
                skillsHealthIncreaseAmount[i].text = (skillsManager.healthIncreasePerLevel * skillsManager.pointsToAddHealth[i]) > 0 
                    ? "+" + skillsManager.healthIncreasePerLevel * skillsManager.pointsToAddHealth[i] : "";

                skillsDefenceIncreaseAmount[i].text = (skillsManager.defenceIncreasePerLevel * skillsManager.pointsToAddDefence[i]) > 0 
                    ? "+" + skillsManager.defenceIncreasePerLevel * skillsManager.pointsToAddDefence[i] : "";

                skillsAttackIncreaseAmount[i].text = (skillsManager.attackIncreasePerLevel * skillsManager.pointsToAddAttack[i]) > 0 
                    ? "+" + skillsManager.attackIncreasePerLevel * skillsManager.pointsToAddAttack[i] : "";

                skillsCritChanceIncreaseAmount[i].text = (skillsManager.critIncreasePerLevel * skillsManager.pointsToAddCritChance[i]) > 0 
                    ? "+" + skillsManager.critIncreasePerLevel * skillsManager.pointsToAddCritChance[i] : "";

                skillsManaIncreaseAmount[i].text = (skillsManager.manaIncreasePerLevel * skillsManager.pointsToAddMana[i]) > 0 
                    ? "+" + skillsManager.manaIncreasePerLevel * skillsManager.pointsToAddMana[i] : "";
                
                skillsCharacterIcons[i].color = Color.white;
                skillsQuestionMarks[i].SetActive(false);
                skillsPartyMemberShrouds[i].SetActive(false);
            }
            else
            { 
                skillsNames[i].text = "Unknown";
                skillsAvailablePoints[i].text = "Available Points: 0";
            
                skillsHealth[i].text = "Health: ???";
                skillsDefence[i].text = "Defence: ???";
                skillsAttack[i].text = "Attack: ???";
                skillsCritChance[i].text = "Crit: ???";
                skillsMana[i].text = "Mana: ???";

                skillsHealthIncrease[i].text = "0";    
                skillsDefenceIncrease[i].text = "0";
                skillsAttackIncrease[i].text = "0";
                skillsCritChanceIncrease[i].text = "0";
                skillsManaIncrease[i].text = "0";
            
                skillsHealthIncreaseAmount[i].text = "";   
                skillsDefenceIncreaseAmount[i].text = ""; 
                skillsAttackIncreaseAmount[i].text = ""; 
                skillsCritChanceIncreaseAmount[i].text = ""; 
                skillsManaIncreaseAmount[i].text = ""; 
                
                skillsCharacterIcons[i].color = Color.black;
                skillsQuestionMarks[i].SetActive(true);
                skillsPartyMemberShrouds[i].SetActive(true);
            }
        }
        SetSkillsButtons();
    }

    private void SetSkillsButtons()
    {
        PartyMember[] partyMembers = skillsManager.partyMembers;

        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (partyMembers[i].IsUnlocked)
            {
                SetAllIncreaseButtons(skillsManager.HasSkillPointsLeft(i), i);
                
                skillsHealthDecreaseButtons[i].interactable = skillsManager.pointsToAddHealth[i] > 0;
                skillsDefenceDecreaseButtons[i].interactable = skillsManager.pointsToAddDefence[i] > 0;
                skillsAttackDecreaseButtons[i].interactable = skillsManager.pointsToAddAttack[i] > 0;
                skillsCritChanceDecreaseButtons[i].interactable = skillsManager.pointsToAddCritChance[i] > 0;
                skillsManaDecreaseButtons[i].interactable = skillsManager.pointsToAddMana[i] > 0;
                
                applySkillPointsButtons[i].interactable = skillsManager.pendingAttributePoints[i] > 0;
            }
            else
            {
                SetAllIncreaseButtons(false, i);
                SetAllDecreaseButtons(false, i);
                applySkillPointsButtons[i].interactable = false;
            }
        }
    }

    private void SetAllIncreaseButtons(bool value, int partyMemberIndex)
    {
        skillsHealthIncreaseButtons[partyMemberIndex].interactable = value;
        skillsDefenceIncreaseButtons[partyMemberIndex].interactable = value;
        skillsAttackIncreaseButtons[partyMemberIndex].interactable = value;
        skillsCritChanceIncreaseButtons[partyMemberIndex].interactable = value;
        skillsManaIncreaseButtons[partyMemberIndex].interactable = value;
    }

    private void SetAllDecreaseButtons(bool value, int partyMemberIndex)
    {
        skillsHealthDecreaseButtons[partyMemberIndex].interactable = value;
        skillsDefenceDecreaseButtons[partyMemberIndex].interactable = value;
        skillsAttackDecreaseButtons[partyMemberIndex].interactable = value;
        skillsCritChanceDecreaseButtons[partyMemberIndex].interactable = value;
        skillsManaDecreaseButtons[partyMemberIndex].interactable = value;
    }

    #endregion

    #region Shop

    private void CloseShop()
    {
        shopScreen.SetActive(false);
        PauseGameManager.Instance.UnPause();
    }

    private void OpenShop(int shopType)
    {
        shopScreen.SetActive(true);
        if(CurrentShopSlot == null) CurrentShopSlot = shopSlotList[0];
        
        if (shopType == 0)
        {
            shopHeading.text = "Supplies Shop";
            shopInventoryTabBox.SetActive(true);
            shopItemQuantityBox.SetActive(true);
            shopEquipmentInventoryHeaderBox.SetActive(false);
            shopEquipmentStatsBox.SetActive(false);
            treasureSellButton.gameObject.SetActive(true);
            SelectShopInventoryTab(0);
        }
        else if (shopType == 1)
        {
            shopHeading.text = "Equipment Shop";
            shopInventoryTabBox.SetActive(false);
            shopItemQuantityBox.SetActive(false);
            shopEquipmentInventoryHeaderBox.SetActive(true);
            shopEquipmentStatsBox.SetActive(true);
            treasureSellButton.gameObject.SetActive(false);
            SelectShopInventoryTab(3);
        }
        
        ShowShopItemDescription(CurrentShopSlot.Index);
        PauseGameManager.Instance.PauseGame();
    }

    private void InitialiseShopInventories()
    {
        for (int i = 0; i < shopInventorySize; i++)
        {
            InventorySlot slot = Instantiate(inventorySlotPrefab, shopItemContainer);
            slot.Index = i;
            shopSlotList.Add(slot);
        }
        
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            InventorySlot slot = Instantiate(inventorySlotPrefab, playerShopItemContainer);
            slot.Index = i + shopInventorySize;
            shopSlotList.Add(slot);
        }
    }
    
    private void ExtraInteractionCallback(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.SuppliesShop:
                OpenShop(0);
                break;
            case InteractionType.EquipmentShop:
                OpenShop(1);
                break;
            case InteractionType.Crafting:
                OpenCloseCraftingPanel(true);
                break;
        }
    }

    public void SelectShopInventoryTab(int index)
    {
        currentShopInventory = index;
        RefreshShop();
        
        foreach (Button tab in shopInventoryTabs)
        {
            ColorBlock colors = tab.colors;    
            colors.normalColor = Color.white;
            tab.colors = colors; 
        }
        Button SelectedTab = shopInventoryTabs[index];
        ColorBlock cb = SelectedTab.colors;    
        cb.normalColor = selectedInventoryColor;
        SelectedTab.colors = cb;
    }
    
    private void DrawPlayerShopInventory(InventoryItem[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            DrawShopItem(items[i], i + shopInventorySize);
        }
    }

    private void DrawNPCShopInventory(InventoryItem[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            DrawShopItem(items[i], i, true);
        }

        for (int i = items.Length; i < shopInventorySize; i++)
        {
            DrawShopItem(null, i, true);
        }
    }
    
    private void DrawShopItem(InventoryItem item, int index, bool isNPCShopItem = false)
    {
        InventorySlot slot = shopSlotList[index];

        slot.ShowSlotInformation(item != null);
        if(item != null) slot.UpdateSlot(item, isNPCShopItem);
    }

    public void RefreshShop()
    {
        DrawPlayerShopInventory(Inventory.Instance.GetInventoryByIndex(currentShopInventory));
        DrawNPCShopInventory(DialogueManager.Instance.NPCSelected.shop);
        ShowShopItemDescription(CurrentShopSlot.Index);
        EventSystem.current.SetSelectedGameObject(CurrentShopSlot.gameObject);
    }

    private void ShowShopItemDescription(int index)
    {
        InventoryItem[] items;
        InventoryItem item;
        bool isBuying = false;
        
        if (index >= shopInventorySize)
        {
            items = Inventory.Instance.GetInventoryByIndex(currentShopInventory);
            item = items[index - shopInventorySize];
        }
        else
        {
            items = DialogueManager.Instance.NPCSelected.shop;
            if (index >= items.Length) item = null;
            else item = items[index];
            isBuying = true;
        }
        
        ShopManager.Instance.SelectItem(item, isBuying);
        
        if (item == null)
        {
            shopItemIcon.gameObject.SetActive(false);
            shopItemName.text = "No Item Selected";
            shopItemDescription.text = "";
            shopItemAmountText.text = "0";
            shopItemPrice.text = "0";
            shopEquipmentStat1.text = "";
            shopEquipmentStat2.text = "";
            
            buySellButton.interactable = false;
            shopDecreaseAmountButton.interactable = false;
            shopIncreaseAmountButton.interactable = false;
            shopMaxAmountButton.interactable = false;
            shopMinAmountButton.interactable = false;
        }
        else
        {
            shopItemIcon.gameObject.SetActive(true);
            shopItemIcon.sprite = item.Icon;
            shopItemName.text = item.Name;
            shopItemDescription.text = item.Description;
            shopItemAmountText.text = ShopManager.Instance.shopItemAmount.ToString();
            shopItemPrice.text = ShopManager.Instance.CalculatePrice(isBuying).ToString();

            if (item is ItemArmour armour)
            {
                shopEquipmentStat1.text = "Health: " + armour.health;
                shopEquipmentStat2.text = "Defence: " + armour.defence;
            }
            else if (item is ItemWeapon weapon)
            {
                shopEquipmentStat1.text = "Damage: " + weapon.damage;
                shopEquipmentStat2.text = "Crit: " + weapon.critChance + "%";
            }
            else if (item is ItemScroll scroll)
            {
                shopEquipmentStat1.text = "Mana: " + scroll.mana;
                shopEquipmentStat2.text = "";
            }
            
            SetShopButtons(item, isBuying);
        }

        shopMinAmountButton.interactable = ShopManager.Instance.shopItemAmount > 1;
        
        bool haveTreasure = inventory.InventoryItemsTreasure.Any(item => item != null);
        treasureSellButton.interactable = haveTreasure;
        if(haveTreasure) treasureSellValue.text = ShopManager.Instance.CalculateAllTreasureValue().ToString();
        else treasureSellValue.text = "0";
    }

    private void SetShopButtons(InventoryItem item, bool isBuying)
    {
        int shopItemAmount = ShopManager.Instance.shopItemAmount;
        
        if (isBuying)
        {
            buySellButtonText.text = "Buy";
            int coinsAmount = CoinManager.Instance.Coins;
            int currentPrice = ShopManager.Instance.CalculatePrice();
            
            shopIncreaseAmountButton.interactable = (coinsAmount >= currentPrice + item.BuyValue) && shopItemAmount < item.MaxStack;
            shopDecreaseAmountButton.interactable = shopItemAmount > 1;
            buySellButton.interactable = coinsAmount >= currentPrice;
            shopMaxAmountButton.interactable = (coinsAmount >= currentPrice + item.BuyValue) && shopItemAmount < item.MaxStack;
        }
        else
        {
            buySellButtonText.text = "Sell";
            
            shopIncreaseAmountButton.interactable = shopItemAmount < Inventory.Instance.GetItemCurrentStock(item.ID);
            shopDecreaseAmountButton.interactable = shopItemAmount > 1;
            buySellButton.interactable = true;
            shopMaxAmountButton.interactable = shopItemAmount < Inventory.Instance.GetItemCurrentStock(item.ID);
        }

        if (item is ItemEquipment equipment)
        {
            buySellButton.interactable = equipment.equipped == -1;
        }
    }

    #endregion

    #region Party
    
    public void UnlockPartyMember(int index)
    {
        partyMembersButtons[index].interactable = true;
        partyMemberImages[index].color = Color.white;
        questionMarks[index].SetActive(false);
        skillsManager.partyMembers[index].UnlockPartyMember();
    }

    public void ResetPartyUnlocks()
    {
        partyMembersButtons[1].interactable = false;
        partyMembersButtons[2].interactable = false;
        partyMemberImages[1].color = Color.black;
        partyMemberImages[2].color = Color.black;
        questionMarks[0].SetActive(true);
        questionMarks[1].SetActive(true);
    }
    
    public void SelectPartyMember(int memberIndex)
    {
        selectedPartyMember = memberIndex;
        UpdateEquipmentList();
        FilterEquipment(currentEquipment);

        foreach (GameObject shroud in partyMemberShrouds)
        {
            shroud.SetActive(true);
        }
        partyMemberShrouds[selectedPartyMember].SetActive(false);
        
        foreach (Button partyMember in partyMembersButtons)
        {
            ColorBlock colors = partyMember.colors;    
            colors.normalColor = selectedPartyMemberColor;
            partyMember.colors = colors; 
        }
        Button SelectedMember = partyMembersButtons[memberIndex];
        ColorBlock cb = SelectedMember.colors;    
        cb.normalColor = Color.white;
        SelectedMember.colors = cb;
    }

    private void SetPartyMemberImages()
    {
        for (int i = 0; i < partyMemberImages.Length; i++)
        {
            partyMemberImages[i].sprite = skillsManager.partyMembers[i].IconFront;
        }

        for (int i = 0; i < skillsManager.partyMembers.Length; i++)
        {
            if (skillsManager.partyMembers[i].IsUnlocked)
            {
                UnlockPartyMember(i);
            }
        }
    }

    private void UpdateCharacterStats()
    {
        PartyMember partyMember = skillsManager.partyMembers[selectedPartyMember];
        characterName.text = partyMember.Name;
        characterDescription.text = partyMember.Description;
        characterHealth.text = "Health: " + partyMember.CurrentMaxHealth;
        characterDefence.text = "Defence: " + partyMember.CurrentDefence;
        characterAttack.text = "Attack: " + partyMember.CurrentAttack;
        characterCritChance.text = "Crit Chance: " + partyMember.CurrentCritChance + "%";
        characterMana.text = "Mana: " + partyMember.CurrentMaxMana;
    }
    
    #endregion

    #region Equipment

    private void SetEquipButtonsInteractable(ItemEquipment selectedItem)
    {
        if (selectedItem == null)
        {
            equipButton.interactable = false;
            unEquipButton.interactable = false;
            return;
        }

        bool isEquipped = selectedItem.equipped != -1;
        bool isEquippedOnSelectedMember = selectedItem.equipped == selectedPartyMember;

        equipButton.interactable = !isEquipped;
        unEquipButton.interactable = isEquipped && isEquippedOnSelectedMember;
    }
    
    private void UpdateEquipmentList()
    {
        ItemEquipment[] items = EquipmentManager.Instance.SortEquipment(currentEquipment);
        DrawEquipmentInventory(items);
        UpdateEquippedItems();
        UpdateCharacterStats();
        SetEquipButtonsInteractable(items[CurrentEquipmentSlot.Index]);
    }

    private void UpdateEquippedItems()
    {
        ItemEquipment[] equippedItems = EquipmentManager.Instance.GetPartyMemberEquipment(selectedPartyMember);
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
            weaponCritHit.text = "Crit: " + weapon.critChance + "%";
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

        if (CurrentEquipmentSlot != null)
        {
            CurrentEquipmentSlot.SetSelected(false);
        }
        
        int currentEquipped = EquipmentManager.Instance.GetEquippedSlotIndex(index, selectedPartyMember);
        if (currentEquipped != -1)
        {
            CurrentEquipmentSlot = equipmentSlotList[currentEquipped];
            CurrentEquipmentSlot.SetSelected(true);
            ShowSelectedEquipment(CurrentEquipmentSlot.Index);
        }
        else
        {
            CurrentEquipmentSlot = equipmentSlotList[0];
            CurrentEquipmentSlot.SetSelected(true);
            ShowSelectedEquipment(0);
        }

        foreach (Button equipmentCard in equipmentCards)
        {
            ColorBlock colors = equipmentCard.colors;    
            colors.normalColor = Color.white;
            equipmentCard.colors = colors; 
        }
        
        Button SelectedCard = equipmentCards[index];
        ColorBlock cb = SelectedCard.colors;    
        cb.normalColor = selectedEquipmentColor;
        SelectedCard.colors = cb;
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
    
    private void ShowSelectedEquipment(int slotIndex)
    {
        ItemEquipment item = EquipmentManager.Instance.SortEquipment(currentEquipment)[slotIndex];
        SetEquipButtonsInteractable(item);
        
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
            selectedItemStat2.text = "Crit: " + weapon.critChance + "%";
        }
        else if (item is ItemScroll scroll)
        {
            selectedItemStat1.text = "Mana: " + scroll.mana;
            selectedItemStat2.text = "";
        }
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
                slot.UpdateSlot(item, skillsManager.partyMembers[armour.equipped].IconFront);
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
                slot.UpdateSlot(item, skillsManager.partyMembers[weapon.equipped].IconFront);
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
                slot.UpdateSlot(item, skillsManager.partyMembers[scroll.equipped].IconFront);
            }
            else
            {
                slot.UpdateSlot(item, null);
            }
        }
        else slot.UpdateSlot(item, null);
    }
    
    private void EquipmentSlotSelectedCallback(int slotIndex)
    {
        CurrentEquipmentSlot.SetSelected(false);
        CurrentEquipmentSlot = equipmentSlotList[slotIndex];
        CurrentEquipmentSlot.SetSelected(true);
        ShowSelectedEquipment(slotIndex);
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
    
    private void InitialiseEquipmentInventory()
    {
        for (int i = 0; i < EquipmentManager.Instance.inventorySize; i++)
        {
            EquipmentSlot slot = Instantiate(equipmentSlotPrefab, equipmentInventoryContainer);
            slot.Index = i;
            equipmentSlotList.Add(slot);
        }
    }
    
    #endregion

    #region Inventory

    public void SwitchInventory(int direction)
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

        ShowItemDescription(CurrentInventorySlot.Index);
        EventSystem.current.SetSelectedGameObject(CurrentInventorySlot.gameObject);
    }
    
    private void DrawInventory(InventoryItem[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            DrawItem(items[i], i);
        }
    }
    
    private void InitialiseInventory()
    {
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            InventorySlot slot = Instantiate(inventorySlotPrefab, inventoryContainer);
            slot.Index = i;
            inventorySlotList.Add(slot);
        }
    }
    
    public void RemoveItem()
    {
        if (CurrentInventorySlot == null)
        {
            return;
        }
        Inventory.Instance.RemoveItem(Inventory.Instance.GetCurrentInventory(),CurrentInventorySlot.Index);
        ShowItemDescription(CurrentInventorySlot.Index);
    }
    
    public void DrawItem(InventoryItem item, int index)
    {
        InventorySlot slot = inventorySlotList[index];

        slot.ShowSlotInformation(item != null);
        if(item != null) slot.UpdateSlot(item);
    }
    
    private void ShowItemDescription(int index)
    {
        InventoryItem[] items = Inventory.Instance.GetCurrentInventory();

        if (items[index] == null)
        {
            itemIcon.gameObject.SetActive(false);
            itemNameTMP.text = "No Item Selected";
            itemDescriptionTMP.text = "";
            destroyButton.interactable = false;
            itemSellValue.text = "0";
        }
        else
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = items[index].Icon;
            itemNameTMP.text = items[index].Name;
            itemDescriptionTMP.text = items[index].Description;
            itemSellValue.text = items[index].SellValue.ToString();
            
            if (items[index] is ItemEquipment equipment)
            {
                destroyButton.interactable = (equipment.equipped == -1);
            }
            else
            {
                destroyButton.interactable = true;
            }
            
            if (currentInventory == questInventoryNumber)
            {
                destroyButton.interactable = false;
            }
        }
    }
    
    private void SlotSelectedCallback(int slotIndex)
    {
        if (tabMenu.activeSelf)
        {
            CurrentInventorySlot.SetSelected(false);
            CurrentInventorySlot = inventorySlotList[slotIndex];
            CurrentInventorySlot.SetSelected(true);
            ShowItemDescription(slotIndex);
        }
        else if (shopScreen.activeSelf)
        {
            CurrentShopSlot.SetSelected(false);
            CurrentShopSlot = shopSlotList[slotIndex];
            CurrentShopSlot.SetSelected(true);
            ShowShopItemDescription(slotIndex);
        }
    }
    
    #endregion

    #region Quests

    public void LoadQuestsUI()
    {
        UpdateQuestList();
    }

    private void UpdateQuestList()
    {
        ClearChildren(questListContent.transform);

        List<Quest> questList = QuestManager.Instance.acceptedQuests;

        Instantiate(questHeaderPrefabs[0], questListContent);   //main
        foreach (Quest quest in questList)
        {
            if(quest.QuestCompleted) continue;
            if (quest.IsMainQuest)
            {
                GameObject newQuest = Instantiate(questPrefab, questListContent);
                newQuest.GetComponent<Button>().onClick.AddListener(() => SelectQuest(quest));
            }
        }
        
        Instantiate(questHeaderPrefabs[1], questListContent);   //side
        foreach (Quest quest in questList)
        {
            if(quest.QuestCompleted) continue;
            if (!quest.IsMainQuest)
            {
                GameObject newQuest = Instantiate(questPrefab, questListContent);
                newQuest.GetComponent<Button>().onClick.AddListener(() => SelectQuest(quest));
            }
        }
        
        Instantiate(questHeaderPrefabs[2], questListContent);   //completed
        foreach (Quest quest in questList)
        {
            if(quest.QuestCompleted)
            {
                GameObject newQuest = Instantiate(questPrefab, questListContent);
                newQuest.GetComponent<Button>().onClick.AddListener(() => SelectQuest(quest));
            }
        }
    }

    private void SelectQuest(Quest quest)
    {
        currentlySelectedQuest = quest;
        
        questTitle.text = quest.Name;
        questDescription.text = quest.Description;
        questGiverIcon.sprite = quest.QuestGiverIcon;
        questGiverName.text = quest.QuestGiverName;
        
        ClearChildren(taskList.transform);

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
        
        UpdateQuestRewards(quest);
    }
    
    private void UpdateQuestRewards(Quest quest)
    {
        ClearChildren(questRewardsList.transform);
         int coinReward = quest.CoinReward;
         int expReward = quest.ExpReward;
         QuestItemReward[] itemRewards = quest.ItemRewards;

         if (coinReward > 0)
         {
             GameObject coinRewardObject = Instantiate(questRewardPrefab, questRewardsList.transform);
             coinRewardObject.GetComponentInChildren<TextMeshProUGUI>().text = "Coins x" + coinReward;
             coinRewardObject.GetComponentInChildren<Image>().sprite = coinIcon;
         }
         if (expReward > 0)
         {
             GameObject expRewardObject = Instantiate(questRewardPrefab, questRewardsList.transform);
             expRewardObject.GetComponentInChildren<TextMeshProUGUI>().text = "Exp x" + expReward;
             expRewardObject.GetComponentInChildren<Image>().sprite = expIcon;
         }
         foreach (QuestItemReward questItemReward in itemRewards)
         {
             InventoryItem item = questItemReward.Item;
             GameObject itemRewardObject = Instantiate(questRewardPrefab, questRewardsList.transform);
             itemRewardObject.GetComponentInChildren<TextMeshProUGUI>().text = item.Name + " x" + questItemReward.Quantity;
             itemRewardObject.GetComponentInChildren<Image>().sprite = item.Icon;
         }
    }
    
    #endregion

    #region Tab Menu
    
    private void OpenCloseTabMenu()
    {
        if(combatManager.isFighting) return;
        if (!SaveLoadManager.Instance.GameIsActive()) return;
        if (shopScreen.activeSelf) return;
        
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
            CloseTabMenu();
        }
    }

    private void CloseTabMenu()
    {
        tabMenu.SetActive(false);
        actions.UI.Disable();
        PauseGameManager.Instance.UnPause();
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

        switch (tabIndex)
        {
            case 0:
                skillsManager.ClearPendingPoints();
                CloseSkillTreeScreen();
                if (selectedSkill == null) selectedSkill = skillsManager.skills[0];
                UpdateSkillsUI();
                break;
            case 1:
                DrawInventory(Inventory.Instance.GetCurrentInventory());
                if (CurrentInventorySlot == null)
                {
                    CurrentInventorySlot = inventorySlotList[0];
                    CurrentInventorySlot.SetSelected(true);
                }
                SelectInventory(currentInventory);
                break;
            case 2:
                FilterEquipment(0);
                UpdateEquippedItems();
                SelectPartyMember(selectedPartyMember);
                SetPartyMemberImages();
                break;
            case 3:
                if (currentlySelectedQuest != null)
                {
                    LoadQuestsUI();
                    SelectQuest(currentlySelectedQuest);
                }
                //TODO set default to main quest
                break;
                
        }
        
        Button SelectedTab = tabButtons[tabIndex];
        ColorBlock cb = SelectedTab.colors;    
        cb.normalColor = selectedTabColor;
        SelectedTab.colors = cb;   
    }

    public void SwitchTab(int direction)
    {
        if (!tabMenu.activeSelf) return;
        
        currentTab += direction;
        if (currentTab < 0) currentTab = tabs.Length - 1;
        if(currentTab >= tabs.Length) currentTab = 0;
        SetTabMenu(currentTab);
    }
    
    #endregion

    #region Start Screen
    
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
        if (SaveLoadManager.Instance.isFirstTimeStartingGame)
        {
            SaveLoadManager.Instance.StartNewGame();
            return;
        }
        HideStartMenu();
        newGameWarning.SetActive(true);
    }

    public void CancelNewGame()
    {
        HideNewGameWarning();
        ShowStartMenu();
    }
    
    public void HideStartMenu()
    {
        startMenu.SetActive(false);
    }

    public void ShowStartMenu()
    {
        startMenu.SetActive(true);
    }
    
    public void HideNewGameWarning()
    {
        newGameWarning.SetActive(false);
    }

    #endregion

    #region Death Screen
    
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

    private void PlayerRespawned()
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
    
    #endregion

    #region Loading

    public void ActivateLoadingScreen(bool isActive)
    {
        loadingScreen.SetActive(isActive);
    }

    public void UpdateLoadingProgress(float progress)
    {
        Vector3 scale = loadingProgressBar.localScale;
        scale.x = progress;
        loadingProgressBar.localScale = scale;
        
        loadingText.text = "Loading - " + (progress * 100) + "%";
    }

    #endregion

    #region Settings
    
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
    
    #endregion
    
    public bool IsInMenu()
    {
        return tabMenu.activeSelf || shopScreen.activeSelf;
    }

    private void CloseMenu()
    {
        if (!IsInMenu()) return;
        
        CloseTabMenu();
        CloseShop();
    }
    
    public void UpdateCoinAmount(int amount)
    {
        playerCoinAmountShop.text = amount.ToString();
        playerCoinAmountInventory.text = amount.ToString();
    }
    
    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void HideGameHUD()
    {
        gameHUD.SetActive(false);
        HidePartyMemberInfo();
    }

    public void ShowGameHUD()
    {
        gameHUD.SetActive(true);
        ShowPartyMemberInfo();
    }
    
    public void CloseAllPanels()
    {
        AudioManager.Instance.PlayButtonPressSound();
        CloseCraftingPanel();
        DialogueManager.Instance.CloseDialoguePanel();
    }
    
    private void CloseCraftingPanel()
    {
        craftingPanel.SetActive(false);
    }

    public void OpenCloseCraftingPanel(bool value)
    {
        CloseAllPanels();
        craftingPanel.SetActive(value);
        CraftingManager.Instance.HideRecipe();
    }
    
    private void OnEnable()
    {
        if (Instance != this) return;
        
        actions.General.Enable();
        
        DialogueManager.OnExtraInteractionEvent += ExtraInteractionCallback;
        InventorySlot.OnSlotSelectedEvent += SlotSelectedCallback;
        EquipmentSlot.OnSlotSelectedEvent += EquipmentSlotSelectedCallback;
    }

    private void OnDisable()
    {
        if (Instance != this) return;
        
        actions.General.Disable();
        actions.UI.Disable();
        
        DialogueManager.OnExtraInteractionEvent -= ExtraInteractionCallback;
        InventorySlot.OnSlotSelectedEvent -= SlotSelectedCallback;
        EquipmentSlot.OnSlotSelectedEvent -= EquipmentSlotSelectedCallback;
    }
    
}
