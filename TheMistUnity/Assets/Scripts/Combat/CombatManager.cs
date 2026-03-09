using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : Singleton<CombatManager>
{
    [SerializeField] private GameObject[] combatTilemaps;
    [SerializeField] private int basicSkillManaRecovery;

    [Header("Ultimate Attack")]
    [SerializeField] private int ultimateFullChargeAmount;
    [SerializeField] private float ultimateAttackTeamUpBonusMultiplier = 1.2f;
    [SerializeField] private float UltimateAttackDamageMultiplier;
    [SerializeField] private float ultimateAttackDamageWait = 0.7f;
    [HideInInspector] public int ultimateCharges;
    [HideInInspector] public int maxUltimateCharges;
    [HideInInspector] public int ultimateChargeProgress;
    [HideInInspector] public int ultimateChargeRingProgress;
    
    [Header("Timing Window")]
    [SerializeField] private float timeBetweenTurns;
    [HideInInspector] public bool isPlayerTurn;
    [HideInInspector] public bool isEnemyTurn;
    private bool[] partyMemberHasTakenTurn;
    private Coroutine enemyTurnCoroutine;

    [Header("Timing Window")]
    [SerializeField] private float timingWindowBaseSpeed;   //pixels per second
    [SerializeField] private float multiHitTimingSpeedBoost;
    [SerializeField] private float perfectAttackMultiplier;
    [SerializeField] private float perfectBlockMultiplier = 0.7f;
    [HideInInspector] public bool isTiming;
    [HideInInspector] public bool perfectTimed;
    private int perfectMultiHits;
    
    private List<EnemyDetails> enemies = new List<EnemyDetails>();
    [HideInInspector] public bool isFighting;
    
    [HideInInspector] public int selectedEnemy;
    [HideInInspector] public int selectedPartyMember;
    
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
    private DialogueManager dialogueManager;
    
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
        dialogueManager = DialogueManager.Instance;

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
        yield return new WaitForSeconds(timeBetweenTurns);
        
        foreach (EnemyDetails enemy in enemies)
        {
            if(enemy.IsDead) continue;
            
            bool finished = false;
            StartCoroutine(enemy.enemyCombatBrain.TakeTurn());
            
            isEnemyTurn = true;
            
            while(isEnemyTurn) yield return null;
            
            yield return new WaitForSeconds(1f);
            
            uIManager.ClearEnemyTargetArrows();
            uIManager.ClearEnemyTurnArrows();
            
            yield return new WaitForSeconds(timeBetweenTurns);
        }

        StartCoroutine(TimeBetweenTurns());
    }

    private void EndEnemyTurn()
    {
        isEnemyTurn = false;
    }

    private void PlayerTurn()
    {
        isPlayerTurn = true;
        uIManager.StartPlayersTurn();
    }
    
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
            StartCoroutine(AttackTargetPartyMember(damage, partyMemberIndex));
        }
    }

    private IEnumerator AttackTargetPartyMember(float damage, int partyMemberIndex)
    {
        uIManager.ShowTimingWindow(false, timingWindowBaseSpeed);
        
        while (isTiming) yield return null;

        if (perfectTimed) damage *= perfectBlockMultiplier;
        
        TakeDamage(Mathf.RoundToInt(damage), partyMemberIndex);
        
        EndEnemyTurn();
    }

    private void TakeDamage(int damage, int partyMemberIndex)
    {
        partyMembersCurrentHealth[partyMemberIndex] -= damage;
        uIManager.ShowPartyMemberCombatText(partyMemberIndex, damage, CombatTextType.Damage);
        
        if (partyMembersCurrentHealth[partyMemberIndex] <= 0)
        {
            partyMembersCurrentHealth[partyMemberIndex] = 0;
            partyMemberIsDead[partyMemberIndex] = true;
            uIManager.KillPartyMember(partyMemberIndex);
            
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
            StartCoroutine(AttackTargetEnemy(attack));
        }
    }
    
    private IEnumerator AttackTargetEnemy(AttackMove attackMove)
    {
        float damage = skillsManager.partyMembers[selectedPartyMember].CurrentAttack * attackMove.DamageMultiplier;
        
        uIManager.ShowTimingWindow(true, timingWindowBaseSpeed);
        
        while (isTiming) yield return null;

        if (perfectTimed) damage *= perfectAttackMultiplier;
        
        DamageEnemy(selectedEnemy, damage, attackMove.DamageType);
        
        StartCoroutine(TimeBetweenTurns());
    }

    private void DamageEnemy(int enemyIndex, float damage, DamageType damageType)
    {
        if (enemies[enemyIndex].TakeDamage(damage, damageType))
        {
            uIManager.KillEnemy(enemyIndex);
            if(AllEnemiesDead()) CombatWin();
        }
        else
        {
            uIManager.SelectEnemy(enemyIndex);
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
    }
    
    public void AddMana(int partyMemberIndex, int amount)
    {
        partyMembersCurrentMana[partyMemberIndex] += amount;
        if(partyMembersCurrentMana[partyMemberIndex] >= skillsManager.partyMembers[partyMemberIndex].CurrentMaxMana) 
            partyMembersCurrentMana[partyMemberIndex] = skillsManager.partyMembers[partyMemberIndex].CurrentMaxMana;
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

    public void FullRecovery()
    {
        for (int i = 0; i < partyMembersCurrentHealth.Length; i++)
        {
            partyMembersCurrentHealth[i] = skillsManager.partyMembers[i].CurrentMaxHealth;
            partyMembersCurrentMana[i] = skillsManager.partyMembers[i].CurrentMaxMana;
        }
        uIManager.UpdatePartyMemberInfo();
    }

    public AttackMove[] GetAllPartyMemberAttacks(int partyMemberIndex)
    {
        ItemEquipment[] equipmentList = equipmentManager.GetPartyMemberEquipment(partyMemberIndex);
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

                if (item.IsRevive)
                {
                    uIManager.RevivePartyMember(i);
                    partyMemberIsDead[i] = false;
                }
                
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
        return (float) ultimateChargeRingProgress / ultimateFullChargeAmount;
    }

    public void UseUltimateAttack()
    {
        StartCoroutine(DealUltimateAttackDamage());
    }

    private IEnumerator DealUltimateAttackDamage()
    {
        bool[] combatSelections = uIManager.combatSelections;
        
        int amountSelected = uIManager.numberOfSelectedPartyMembers;
        UseUltimateCharges(amountSelected);
        uIManager.ExitCombatSelectionScreen();
        uIManager.StartEnemyTurn();

        for (int i = 0; i < combatSelections.Length; i++)
        {
            if (combatSelections[i])
            {
                float damage = skillsManager.partyMembers[i].CurrentAttack * UltimateAttackDamageMultiplier;
                List<DamageType> damageTypes = equipmentManager.GetPartyMemberDamageTypes(i);

                for (int j = 0; j < amountSelected; j++)
                {
                    if (j >= 1)
                    {
                        damage *= ultimateAttackTeamUpBonusMultiplier;
                    }
                }
                
                while (damageTypes.Count < 2)
                {
                    DamageType none = ScriptableObject.CreateInstance<DamageType>();
                    none.damageType = DamageTypes.None;
                    damageTypes.Add(none);
                }

                int livingEnemies = 0;
                foreach (EnemyDetails enemy in enemies)
                {
                    if(enemy.IsDead) continue;
                    livingEnemies++;
                }
                
                foreach (DamageType damageType in damageTypes)
                {
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        if(enemies[j].IsDead) continue;
                        DamageEnemy(j, damage / livingEnemies, damageType);
                    }
                    
                    uIManager.UpdateEnemyHealthBar();
                    yield return new WaitForSeconds(ultimateAttackDamageWait);
                }
            }
        }
        
        StartCoroutine(TimeBetweenTurns());
    }

    private void UseUltimateCharges(int amount)
    {
        ultimateCharges -= amount;
        AddUltimateCharge(0);
        uIManager.UpdateUltimateCharges();
    }

    [SerializeField] private int perfectTimingChargeAmount;
    
    public void GetPerfectTimingCharge()
    {
        float amount = perfectTimingChargeAmount;

        if (skillsManager.GetSkill(SkillTreeSkills.IncreaseUltimateChargeSpeed).IsUnlocked)
        {
            amount *= skillsManager.ultimateChargeSpeedIncreaseMultiplier;
        }
        Debug.Log(amount);
        AddUltimateCharge(Mathf.RoundToInt(amount));
        uIManager.UpdateUltimateCharges();
    }

    public void AddUltimateCharge(int chargeAmount)
    {
        
        ultimateChargeProgress += chargeAmount;
        ultimateChargeRingProgress = ultimateChargeProgress;

        if (ultimateCharges == maxUltimateCharges)
        {
            ultimateChargeProgress = 0;
            ultimateChargeRingProgress = ultimateFullChargeAmount;
        }

        if (ultimateChargeProgress < ultimateFullChargeAmount) return;
        
        if (ultimateCharges < maxUltimateCharges - 1)
        {
            ultimateCharges++;
            ultimateChargeProgress -= ultimateFullChargeAmount;
            ultimateChargeRingProgress = ultimateChargeProgress;
        }
        else
        {
            if (ultimateCharges < maxUltimateCharges) ultimateCharges++;
            ultimateChargeRingProgress = ultimateFullChargeAmount;
            ultimateChargeProgress = 0;
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
        if(enemyTurnCoroutine != null) StopCoroutine(enemyTurnCoroutine);
        usedItems.Clear();
        LoadCombatData();
    }

    private void CombatWin()
    {
        UseItems();
        AddRewards();
        //PostBattleRecovery();
        EndCombat();
        SaveCombatData();
    }

    private void AddRewards()
    {
        foreach (EnemyDetails enemy in enemies)
        {
            float coinAmount = enemy.CoinsReward;
            if (skillsManager.GetSkill(SkillTreeSkills.BonusCombatGold).IsUnlocked)
            {
                coinAmount *= skillsManager.coinIncreaseMultiplier;
            }
            coinManager.AddCoins(Mathf.RoundToInt(coinAmount));

            float expAmount = enemy.ExpReward;
            if (skillsManager.GetSkill(SkillTreeSkills.BonusCombatExp).IsUnlocked)
            {
                expAmount *= skillsManager.expIncreaseMultiplier;
            }
            skillsManager.AddExp(Mathf.RoundToInt(expAmount));
        }
    }

    private void EndCombat()
    {
        gameManager.EnablePlayerMovement();
        gameManager.Player.gameObject.SetActive(true);
        uIManager.DeactivateCombatScreen();
        cameraManager.ToggleCombatCamera();
        dialogueManager.SelectNPC(null);
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
    }

    private void RevivePartyMembers()
    {
        for (int i = 0; i < partyMemberIsDead.Length; i++)
        {
            partyMemberIsDead[i] = false;
            AddHealth(i, 1);
        }
        uIManager.UpdatePartyMemberInfo();
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
