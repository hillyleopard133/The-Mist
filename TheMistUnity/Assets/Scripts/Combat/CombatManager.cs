using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : Singleton<CombatManager>
{
    [SerializeField] private GameObject[] combatTilemaps;
    [SerializeField] private float timeBetweenTurns;

    private List<EnemyDetails> enemies = new List<EnemyDetails>();
    private bool isFighting;
    
    [HideInInspector] public int selectedEnemy;
    [HideInInspector] public int selectedPartyMember;

    [HideInInspector] public int ultimateCharges;
    [HideInInspector] public int maxUltimateCharges;
    [HideInInspector] public int ultimateChargeProgress;
    [SerializeField] private int ultimateFullChargeAmount;
    
    private List<InventoryItem> usedItems = new List<InventoryItem>();
    private const int consumableInventoryIndex = 2;

    //To update and save after each combat win
    private int[] partyMembersCurrentHealth;
    private int[] partyMembersCurrentMana;
    
    private CameraManager cameraManager;
    private SkillsManager skillsManager;
    private UIManager uIManager;
    private GameManager gameManager;
    private Inventory inventory;
    private CoinManager coinManager;
    private EquipmentManager equipmentManager;
    
    private readonly string ULTIMATE_CHARGES = "ULTIMATE_CHARGES";
    private readonly string ULTIMATE_CHARGE_PROGRESS = "ULTIMATE_CHARGE_PROGRESS";
    private readonly string PARTY_MEMBER_CURRENT_HEALTH = "PARTY_MEMBER_CURRENT_HEALTH";
    private readonly string PARTY_MEMBER_CURRENT_MANA= "PARTY_MEMBER_CURRENT_MANA";
    
    private void Start()
    {
        cameraManager = CameraManager.Instance;
        skillsManager = SkillsManager.Instance;
        uIManager = UIManager.Instance;
        gameManager = GameManager.Instance;
        inventory = Inventory.Instance;
        equipmentManager = EquipmentManager.Instance;
        
        partyMembersCurrentHealth = new int[skillsManager.partyMembers.Length];
        partyMembersCurrentMana = new int[skillsManager.partyMembers.Length];
    }

    public int GetPartyMemberCurrentHealth(int index)
    {
        return partyMembersCurrentHealth[index];
    }
    
    public float GetPartyMemberCurrentHealthPercentage(int index)
    {
        return (float) partyMembersCurrentHealth[index] / skillsManager.partyMembers[index].CurrentMaxHealth;
    }
    
    public float GetPartyMemberHealthRecoveryPercentage(int index, ItemConsumable consumable)
    {
        int healthTotal = partyMembersCurrentHealth[index] + consumable.GetHealthValue();
        int maxHealth = skillsManager.partyMembers[index].CurrentMaxHealth;
        
        if(healthTotal >= maxHealth) return 1;
        
        return (float) healthTotal / maxHealth;
    }
    
    public int GetPartyMemberCurrentMana(int index)
    {
        return partyMembersCurrentMana[index];
    }
    
    public float GetPartyMemberCurrentManaPercentage(int index)
    {
        return (float) partyMembersCurrentMana[index] / skillsManager.partyMembers[index].CurrentMaxMana;
    }
    
    public float GetPartyMemberManaRecoveryPercentage(int index, ItemConsumable consumable)
    {
        int manaTotal = partyMembersCurrentMana[index] + consumable.GetManaValue();
        int maxMana = skillsManager.partyMembers[index].CurrentMaxMana;
        
        if(manaTotal >= maxMana) return 1;
        
        return (float) manaTotal / maxMana;
    }

    public AttackMove[] GetAllPartyMemberAttacks(int partyMemberIndex)
    {
        ItemEquipment[] equipmentList = equipmentManager.GetCharacterEquipment(partyMemberIndex);
        List<AttackMove> unlockedAttacks = skillsManager.partyMembers[partyMemberIndex].GetUnlockedAttacks();

        List<AttackMove> attacks = new List<AttackMove>();
        attacks.AddRange(unlockedAttacks);

        ItemWeapon weapon = null;
        ItemScroll scroll = null;
        foreach (ItemEquipment equipment in equipmentList)
        {
            if(equipment == null) continue;
            if (equipment is ItemWeapon w) weapon = w;
            else if (equipment is ItemScroll s) scroll = s;
        }
        
        if(weapon != null) attacks.AddRange(weapon.Attacks);
        if (scroll != null) attacks.AddRange(scroll.Attacks);
        
        return attacks.ToArray();
    }
    
    public void TestUltimateCharge()
    {
        AddUltimateCharge(120);
    }

    public int GetItemAmountLeft(string itemID)
    {
        int amount = inventory.GetItemCurrentStock(itemID);

        foreach (InventoryItem item in usedItems)
        {
            if(itemID == item.ID) amount--;
        }
        
        return amount;
    }

    public void UseItem()
    {
        ItemConsumable item = uIManager.selectedCombatItem;
        usedItems.Add(item);

        if (item.IsWholeParty)
        {
            //TODO add items effect
            // uiManager.combatSelections[]
        }
        else
        {
            
        }
        
        uIManager.OpenCombatInventory();
        uIManager.ExitCombatSelectionScreen();
    }

    public int NumberOfEnemies()
    {
        return enemies.Count;
    }

    public float GetUltimateChargeProgressPercentage()
    {
        return (float) ultimateChargeProgress / ultimateFullChargeAmount;
    }

    public void UseUltimateAttack()
    {
        bool[] combatSelections = uIManager.combatSelections;
        
    }

    private void UseUltimateCharges(int amount)
    {
        ultimateCharges -= amount;
        uIManager.UpdateUltimateCharges();
    }

    private void AddUltimateCharge(int chargeAmount)
    {
        ultimateChargeProgress += chargeAmount;

        if (ultimateChargeProgress < ultimateFullChargeAmount) return;
        
        if (ultimateCharges < maxUltimateCharges - 1)
        {
            ultimateCharges++;
            ultimateChargeProgress -= ultimateFullChargeAmount;
        }
        else
        {
            if (ultimateCharges < maxUltimateCharges) ultimateCharges++; 
            ultimateChargeProgress = ultimateFullChargeAmount;
        }
        
        uIManager.UpdateUltimateCharges();
    }

    public int GetMaxUltimateCharges()
    {
        int chargeAmount = 1;
        
        if(skillsManager.GetSkill(SkillTreeSkills.ExtraUltimateCharge1).IsUnlocked) chargeAmount++;
        if(skillsManager.GetSkill(SkillTreeSkills.ExtraUltimateCharge2).IsUnlocked) chargeAmount++;
        if(skillsManager.GetSkill(SkillTreeSkills.ExtraUltimateCharge3).IsUnlocked) chargeAmount++;
        
        maxUltimateCharges = chargeAmount;
        
        return chargeAmount;
    }

    public EnemyDetails GetSelectedEnemy()
    {
        return enemies[selectedEnemy];
    }

    public void EnterCombat(List<EnemyDetails> enemies, CombatGridType gridType)
    {
        this.enemies = enemies;
        gameManager.DisablePlayerMovement();
        gameManager.Player.gameObject.SetActive(false);
        uIManager.ActivateCombatScreen(enemies);
        cameraManager.ToggleCombatCamera();
        combatTilemaps[(int)gridType].SetActive(true);
        isFighting = true;
        AddUltimateCharge(0);
    }

    private void CombatLose()
    {
        EndCombat();
        usedItems.Clear();
    }

    private void CombatWin()
    {
        EndCombat();
        UseItems();
        AddRewards();
    }

    private void AddRewards()
    {
        foreach (EnemyDetails enemy in enemies)
        {
            coinManager.AddCoins(enemy.CoinsReward);
            skillsManager.AddExp(enemy.ExpReward);
        }
    }

    private void EndCombat()
    {
        gameManager.EnablePlayerMovement();
        gameManager.Player.gameObject.SetActive(true);
        uIManager.DeactivateCombatScreen();
        cameraManager.ToggleCombatCamera();
        DeactivateTilemaps();
        isFighting = false;
        SaveCombatData();
    }

    private void DeactivateTilemaps()
    {
        foreach (GameObject tilemap in combatTilemaps)
        {
            tilemap.SetActive(false);
        }
    }

    private void UseItems()
    {
        foreach (InventoryItem item in usedItems)
        {
            inventory.ConsumeItem(item.ID);
        }
        usedItems.Clear();
    }

    public void SaveCombatData()
    {
        SaveGame.Save(PARTY_MEMBER_CURRENT_MANA, partyMembersCurrentMana);
        SaveGame.Save(PARTY_MEMBER_CURRENT_HEALTH, partyMembersCurrentHealth);
        SaveGame.Save(ULTIMATE_CHARGES, ultimateCharges);
        SaveGame.Save(ULTIMATE_CHARGE_PROGRESS, ultimateChargeProgress);
    }

    public void LoadCombatData()
    {
        if(SaveGame.Exists(PARTY_MEMBER_CURRENT_MANA)) partyMembersCurrentMana = SaveGame.Load<int[]>(PARTY_MEMBER_CURRENT_MANA);
        if(SaveGame.Exists(PARTY_MEMBER_CURRENT_HEALTH)) partyMembersCurrentHealth = SaveGame.Load<int[]>(PARTY_MEMBER_CURRENT_HEALTH);
        if (SaveGame.Exists(ULTIMATE_CHARGES)) ultimateCharges = SaveGame.Load<int>(ULTIMATE_CHARGES);
        if (SaveGame.Exists(ULTIMATE_CHARGE_PROGRESS)) ultimateChargeProgress = SaveGame.Load<int>(ULTIMATE_CHARGE_PROGRESS);
    }

    public void ResetCombatData()
    {
        ultimateCharges = 0;
        ultimateChargeProgress = 0;

        PartyMember[] partyMembers = skillsManager.partyMembers;
        for (int i = 0; i < partyMembers.Length; i++)
        {
            partyMembersCurrentHealth[i] = partyMembers[i].BaseMaxHealth;
            partyMembersCurrentMana[i] = partyMembers[i].BaseMaxMana;
        }
        
        SaveCombatData();
    }
}
