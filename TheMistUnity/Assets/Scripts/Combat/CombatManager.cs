using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : Singleton<CombatManager>
{
    [SerializeField] private GameObject[] combatTilemaps;
    [SerializeField] private float timeBetweenTurns;

    private List<InventoryItem> usedItems = new List<InventoryItem>();
    private List<EnemyDetails> enemies = new List<EnemyDetails>();
    private bool isFighting;
    
    private CameraManager cameraManager;
    private SkillsManager skillsManager;
    private UIManager uIManager;
    private GameManager gameManager;
    private Inventory inventory;
    private CoinManager coinManager;

    [HideInInspector] public int selectedEnemy;
    [HideInInspector] public int selectedPartyMember;

    [HideInInspector] public int ultimateCharges;
    [HideInInspector] public int maxUltimateCharges;
    [HideInInspector] public int ultimateChargeProgress;
    [SerializeField] private int ultimateFullChargeAmount;

    //To update and save after each combat win
    private int[] partyMembersCurrentHealth;
    private int[] partyMembersCurrentMana;
    
    private readonly string ULTIMATE_CHARGES = "ULTIMATE_CHARGES";
    private readonly string ULTIMATE_CHARGE_PROGRESS = "ULTIMATE_CHARGE_PROGRESS";
    
    private void Start()
    {
        cameraManager = CameraManager.Instance;
        skillsManager = SkillsManager.Instance;
        uIManager = UIManager.Instance;
        gameManager = GameManager.Instance;
        inventory = Inventory.Instance;
    }
    
    public void TestUltimateCharge()
    {
        AddUltimateCharge(120);
    }

    public int NumberOfEnemies()
    {
        return enemies.Count;
    }

    public float GetUltimateChargeProgressPercentage()
    {
        return (float) ultimateChargeProgress / ultimateFullChargeAmount;
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
        SaveGame.Save(ULTIMATE_CHARGES, ultimateCharges);
        SaveGame.Save(ULTIMATE_CHARGE_PROGRESS, ultimateChargeProgress);
    }

    public void LoadCombatData()
    {
        if (SaveGame.Exists(ULTIMATE_CHARGES)) ultimateCharges = SaveGame.Load<int>(ULTIMATE_CHARGES);
        if (SaveGame.Exists(ULTIMATE_CHARGE_PROGRESS)) ultimateChargeProgress = SaveGame.Load<int>(ULTIMATE_CHARGE_PROGRESS);
    }

    public void ResetCombatData()
    {
        ultimateCharges = 0;
        ultimateChargeProgress = 0;
        
        SaveCombatData();
    }
}
