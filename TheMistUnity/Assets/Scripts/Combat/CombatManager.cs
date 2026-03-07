using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : Singleton<CombatManager>
{
    [SerializeField] private GameObject[] combatTilemaps;

    private List<EnemyDetails> enemies = new List<EnemyDetails>();
    [HideInInspector] public bool isFighting;
    
    [HideInInspector] public int selectedEnemy;
    [HideInInspector] public int selectedPartyMember;

    [HideInInspector] public int ultimateCharges;
    [HideInInspector] public int maxUltimateCharges;
    [HideInInspector] public int ultimateChargeProgress;
    [SerializeField] private int ultimateFullChargeAmount;
    
    [SerializeField] private int basicSkillManaRecovery;
    [SerializeField] private float timeBetweenTurns;
    [HideInInspector] public bool isPlayerTurn;
    private bool[] partyMemberHasTakenTurn;
    
    private List<InventoryItem> usedItems = new List<InventoryItem>();
    
    private int[] partyMembersCurrentHealth;
    private int[] partyMembersCurrentMana;
    private bool[] partyMemberIsDead;
    
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
        coinManager = CoinManager.Instance;
        equipmentManager = EquipmentManager.Instance;

        int partySize = skillsManager.partyMembers.Length;
        
        partyMembersCurrentHealth = new int[partySize];
        partyMembersCurrentMana = new int[partySize];
        partyMemberIsDead = new bool[partySize];
        partyMemberHasTakenTurn = new bool[partySize];
    }

    public void TestEndCombat()
    {
        CombatLose();
    }
    
    public void TestUltimateCharge()
    {
        AddUltimateCharge(120);
    }
    
    private IEnumerator EnemyTurn()
    {
        isPlayerTurn = false;

        foreach (EnemyDetails enemy in enemies)
        {
            if(enemy.IsDead) continue;
            
            bool finished = false;
            StartCoroutine(enemy.enemyCombatBrain.TakeTurn(() => finished = true));
            while(!finished) yield return null;
            
            yield return new WaitForSeconds(timeBetweenTurns);
        }

        StartCoroutine(TimeBetweenTurns());
    }

    private void PlayerTurn()
    {
        isPlayerTurn = true;
        uIManager.StartPlayersTurn();
    }

    private Coroutine enemyTurnCoroutine;
    private IEnumerator TimeBetweenTurns()
    {
        if (isPlayerTurn)
        {
            uIManager.StartEnemyTurn();
            partyMemberHasTakenTurn[selectedPartyMember] = true;
            
            bool allTurnsMade = true;
            for (int i = 0; i < partyMemberHasTakenTurn.Length; i++)
            {
                if (!partyMemberHasTakenTurn[i] && !partyMemberIsDead[i] && skillsManager.partyMembers[i].IsUnlocked)
                {
                    uIManager.SelectCombatPartyMember(i);
                    allTurnsMade = false;
                    break;
                }
            }

            if (allTurnsMade)
            {
                for (int i = 0; i < partyMemberHasTakenTurn.Length; i++)
                {
                    partyMemberHasTakenTurn[i] = false;
                }
            }
        } 
        
        yield return new WaitForSeconds(timeBetweenTurns);
        if (isFighting)
        {
            if (isPlayerTurn)
            {
                enemyTurnCoroutine = StartCoroutine(EnemyTurn());
            }
            else
            {
                PlayerTurn();
            }
        }
    }

    public bool HasTakenTurn(int partyMemberIndex)
    {
        return partyMemberHasTakenTurn[partyMemberIndex];
    }

    public void AttackPartyMember(int partyMemberIndex, AttackMove attackMove, EnemyDetails enemyDetails)
    {
        float damage = enemyDetails.AttackDamage * attackMove.DamageMultiplier;

        if (attackMove.MoveType == AttackMoveType.SingleTarget)
        {
            TakeDamage(Mathf.RoundToInt(damage), partyMemberIndex);
        }
    }

    private void TakeDamage(int damage, int partyMemberIndex)
    {
        partyMembersCurrentHealth[partyMemberIndex] -= damage;
        uIManager.ShowPartyMemberCombatText(partyMemberIndex, damage, CombatTextType.Damage);
        
        if (partyMembersCurrentHealth[partyMemberIndex] <= 0)
        {
            partyMembersCurrentHealth[partyMemberIndex] = 0;
            partyMemberIsDead[partyMemberIndex] = true;
            
            int allDead = AllPartyMembersDead();
            if (allDead != -1)
            {
                uIManager.SelectCombatPartyMember(allDead);
            }
            else
            {
                CombatLose();
            }
        }
        
        uIManager.UpdatePartyMemberInfo();
    }
    
    private int AllPartyMembersDead()
    {
        for (int i = 0; i < partyMemberIsDead.Length; i++)
        {
            if (!partyMemberIsDead[i] && skillsManager.partyMembers[i].IsUnlocked)
            {
                return i;
            }
        }
        return -1;
    }

    public void AttackEnemy(AttackMove attack)
    {
        if (attack.Type == AttackType.Basic)
        {
            int recoveryAmount = basicSkillManaRecovery;
            int currentMana = partyMembersCurrentMana[selectedPartyMember];
            int maxMana = skillsManager.partyMembers[selectedPartyMember].CurrentMaxMana;

            if (currentMana + recoveryAmount > maxMana)
            {
                recoveryAmount = maxMana - currentMana;
            }
            
            partyMembersCurrentMana[selectedPartyMember] += recoveryAmount;
            if(recoveryAmount > 0) uIManager.ShowPartyMemberCombatText(selectedPartyMember, recoveryAmount, CombatTextType.ManaRecovery);
        }
        else if(attack.Type == AttackType.Skill) partyMembersCurrentMana[selectedPartyMember] -= attack.MPCost;
        
        uIManager.UpdatePartyMemberInfo();
        
        if (attack.MoveType == AttackMoveType.SingleTarget)
        {
            AttackTargetEnemy(attack);
        }
        
        StartCoroutine(TimeBetweenTurns());
    }

    private void AttackTargetEnemy(AttackMove attackMove)
    {
        float damage = skillsManager.partyMembers[selectedPartyMember].CurrentAttack * attackMove.DamageMultiplier;

        if (enemies[selectedEnemy].TakeDamage(damage, attackMove.DamageType))
        {
            uIManager.KillEnemy(selectedEnemy);
            if(AllEnemiesDead()) CombatWin();
        }
        else
        {
            uIManager.SelectEnemy(selectedEnemy);
        }
    }

    private bool AllEnemiesDead()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].IsDead)
            {
                uIManager.SelectEnemy(i);
                return false;
            }
        }
        return true;
    }

    public int GetHighestHPPartyMember()
    {
        int health = 0;
        int highestIndex = 0;
        
        for (int i = 0; i < partyMembersCurrentHealth.Length; i++)
        {
            if (partyMembersCurrentHealth[i] > health && skillsManager.partyMembers[i].IsUnlocked)
            {
                health = partyMembersCurrentHealth[i];
                highestIndex = i;
            } 
        }

        return highestIndex;
    }

    public void LevelUp()
    {
        for (int i = 0; i < partyMembersCurrentHealth.Length; i++)
        {
            partyMembersCurrentHealth[i] = skillsManager.partyMembers[i].CurrentMaxHealth;
            partyMembersCurrentMana[i] = skillsManager.partyMembers[i].CurrentMaxMana;
            partyMemberIsDead[i] = false;
        }
        uIManager.UpdatePartyMemberInfo();
        SaveCombatData();
    }

    public void AddHealth(int partyMemberIndex, int amount)
    {
        partyMembersCurrentHealth[partyMemberIndex] += amount;
        if(partyMembersCurrentHealth[partyMemberIndex] >= skillsManager.partyMembers[partyMemberIndex].CurrentMaxHealth)
            partyMembersCurrentHealth[partyMemberIndex] = skillsManager.partyMembers[partyMemberIndex].CurrentMaxHealth;
        SaveCombatData();
    }
    
    public void AddMana(int partyMemberIndex, int amount)
    {
        partyMembersCurrentMana[partyMemberIndex] += amount;
        if(partyMembersCurrentMana[partyMemberIndex] >= skillsManager.partyMembers[partyMemberIndex].CurrentMaxMana) 
            partyMembersCurrentMana[partyMemberIndex] = skillsManager.partyMembers[partyMemberIndex].CurrentMaxMana;
        SaveCombatData();
    }

    public bool IsPartyMemberDead(int index)
    {
        return partyMemberIsDead[index];
    }

    public bool IsEnemyDead(int index)
    {
        return enemies[index].IsDead;
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

        bool[] selectedParty = uIManager.combatSelections;

        for (int i = 0; i < selectedParty.Length; i++)
        {
            if (selectedParty[i])
            {
                if(partyMemberIsDead[i] && !item.IsRevive) continue;

                if(item.IsRevive) partyMemberIsDead[i] = false;
                
                int healthValue = item.GetHealthValue();
                partyMembersCurrentHealth[i] += healthValue;
                if(partyMembersCurrentHealth[i] >= skillsManager.partyMembers[i].CurrentMaxHealth) 
                    partyMembersCurrentHealth[i] = skillsManager.partyMembers[i].CurrentMaxHealth;
                
                int manaValue = item.GetManaValue();
                partyMembersCurrentMana[i] += manaValue;
                if(partyMembersCurrentMana[i] >= skillsManager.partyMembers[i].CurrentMaxMana) 
                    partyMembersCurrentMana[i] = skillsManager.partyMembers[i].CurrentMaxMana;

                if (manaValue > 0 && healthValue > 0)
                {
                    StartCoroutine(QueueRecoveryText(i, manaValue, healthValue));
                }
                else if (manaValue > 0) uIManager.ShowPartyMemberCombatText(i, manaValue, CombatTextType.ManaRecovery);
                else if (healthValue > 0) uIManager.ShowPartyMemberCombatText(i, healthValue, CombatTextType.HealthRecovery);
            }
        }
        uIManager.UpdatePartyMemberInfo();
        uIManager.OpenCombatInventory();
        uIManager.ExitCombatSelectionScreen();
        
        StartCoroutine(TimeBetweenTurns());
    }

    private IEnumerator QueueRecoveryText(int partyMemberIndex, int manaValue, int healthValue)
    {
        uIManager.ShowPartyMemberCombatText(partyMemberIndex, healthValue, CombatTextType.HealthRecovery);
        yield return new WaitForSeconds(0.3f);
        uIManager.ShowPartyMemberCombatText(partyMemberIndex, manaValue, CombatTextType.ManaRecovery);
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
        int amountSelected = uIManager.numberOfSelectedPartyMembers;

        UseUltimateCharges(amountSelected);
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
        SaveCombatData();
        this.enemies = enemies;
        gameManager.DisablePlayerMovement();
        gameManager.Player.gameObject.SetActive(false);
        uIManager.ActivateCombatScreen(enemies);
        cameraManager.ToggleCombatCamera();
        combatTilemaps[(int)gridType].SetActive(true);
        isFighting = true;
        isPlayerTurn = false;
        StartCoroutine(TimeBetweenTurns());
        AddUltimateCharge(0);
    }

    private void CombatLose()
    {
        EndCombat();
        StopCoroutine(enemyTurnCoroutine);
        usedItems.Clear();
        LoadCombatData();
    }

    private void CombatWin()
    {
        UseItems();
        AddRewards();
        EndCombat();
        PostBattleRecovery();
        SaveCombatData();
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
        RevivePartyMembers();
        isFighting = false;
    }

    private void PostBattleRecovery()
    {
        for (int i = 0; i < partyMembersCurrentHealth.Length; i++)
        {
            AddHealth(i, 20);
            AddMana(i, 5);
        }
        uIManager.UpdatePartyMemberInfo();
    }

    private void RevivePartyMembers()
    {
        for (int i = 0; i < partyMemberIsDead.Length; i++)
        {
            partyMemberIsDead[i] = false;
        }
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
        
        uIManager.UpdatePartyMemberInfo();
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
