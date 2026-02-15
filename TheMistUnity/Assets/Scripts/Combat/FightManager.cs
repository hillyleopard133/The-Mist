using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SandwichFightManager : Singleton<SandwichFightManager>
{
    [SerializeField] private float timeBetweenTurns;
    private Player player;

    [Header("Effect Config")] 
    [SerializeField] private int poisonDamageAmount = 6;
    [SerializeField] private Transform playerInflictedEffectsContainer;
    [SerializeField] private Transform enemyInflictedEffectsContainer;
    [SerializeField] private GameObject inflictedEffectPrefab;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject playerAttackButtonsContent;
    [SerializeField] private GameObject playerAttackButtonsPanelBlocker;
    [SerializeField] private GameObject fightScreen;
    [SerializeField] private GameObject attackButtonPrefab;
    [SerializeField] private TextMeshProUGUI catchChance;
    [SerializeField] private GameObject newSandwichOptionsPanel;

    [Header("Sandwich info panels")]
    [SerializeField] private Image playerSandwichImage;
    [SerializeField] private Image enemySandwichImage;
    [SerializeField] private TextMeshProUGUI enemyHealth;
    [SerializeField] private TextMeshProUGUI playerHealth;
    [SerializeField] private TextMeshProUGUI enemyRarity;
    [SerializeField] private TextMeshProUGUI playerRarity;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI enemyName;
    
    [Header("Attack move description")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI moveName;
    [SerializeField] private TextMeshProUGUI effectType;
    [SerializeField] private TextMeshProUGUI effectDuration;
    [SerializeField] private TextMeshProUGUI effectChance;
    [SerializeField] private Image effectIcon;
    [SerializeField] private TextMeshProUGUI damageAmount;
    [SerializeField] private TextMeshProUGUI hitChance;
    
    [Header("Battle log")]
    [SerializeField] private Transform battleLogContainer;
    [SerializeField] private GameObject battleLogPrefab;
    [SerializeField] private ScrollRect battleLogScrollRect;
    
    private EnemyCombat enemy;

    private bool turnInProgress;
    private bool isPlayerTurn;
    [HideInInspector] public bool isFighting;

    protected override void Awake()
    {
        base.Awake();
        fightScreen.SetActive(false);
        descriptionPanel.SetActive(false);
    }

    /*
    private void UpdateInflictedEffectsUI()
    {
        UpdateInflictedEffectsUI(player, playerInflictedEffectsContainer);
        UpdateInflictedEffectsUI(enemy, enemyInflictedEffectsContainer);
    }

    private void UpdateInflictedEffectsUI(SandwichMonster sandwich, Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        
        foreach (AttackEffect effect in sandwich.GetInflictedEffects())
        {
            if (effect == null) continue;
            
            GameObject instantiatedEffect = Instantiate(inflictedEffectPrefab, container);
            instantiatedEffect.GetComponent<Image>().sprite = effect.GetIcon();
            instantiatedEffect.GetComponentInChildren<TextMeshProUGUI>().text = effect.GetEffectDuration().ToString();
        }
    }

    public SandwichMonster GetEnemy()
    {
        return enemy;
    }

    public SandwichMonster GetPlayer()
    {
        return player;
    }

    public void SetPlayer(SandwichMonster sandwich)
    {
        player = sandwich;
        BuildPlayerAttackButtons();
    }

    public void ShowAttackMoveDescription(SandwichMonsterAttackMove attack)
    {
        descriptionPanel.SetActive(true);
        moveName.text = attack.GetMoveName();
        damageAmount.text = attack.GetDamage().ToString();
        hitChance.text = attack.GetHitChance().ToString();
        
        if (attack.HasEffect())
        {
            effectIcon.gameObject.SetActive(true);
            effectChance.gameObject.SetActive(true);
            effectDuration.gameObject.SetActive(true);
            AttackEffect effect = attack.GetEffect();
            effectType.text = effect.GetEffectType().ToString();
            effectIcon.sprite = effect.GetIcon();
            effectDuration.text = "Duration: " + effect.GetEffectDuration();
            effectChance.text = "Chance: " + attack.GetEffectChance() + "%";
            
        }
        else
        {
            effectIcon.gameObject.SetActive(false);
            effectDuration.gameObject.SetActive(false);
            effectChance.gameObject.SetActive(false);
            effectType.text = "None";
        }
    }

    public void HideAttackMoveDescription()
    {
        descriptionPanel.SetActive(false);
    }

    public void KillSandwich(SandwichMonster sandwich)
    {
        if (sandwich == enemy)
        {
            CoinManager.Instance.AddCoins(sandwich.GetMaxHealth() / 5);
            SandwichMonsterManager.Instance.AddKill(enemy.GetMonsterName());
            EndBattle();
        }else if (sandwich == player)
        {
            if (PlayerSandwichTeam.Instance.GetFirstLiveSandwich() != null)
            {
                SwapActiveSandwich();
            }
            else
            {
                EndBattle();
            }
            
        }
    }

    public void CatchSandwich()
    {
        float chance = 100 - enemy.GetHealthPercentage() - 20;
        int random = Random.Range(0, 100);
        if (random <= chance)
        {
            AddToBattleLog("Caught a " + enemy.GetMonsterName() + "!");
            SandwichMonsterManager.Instance.AddCapture(enemy.GetMonsterName());
            newSandwichOptionsPanel.SetActive(true);
            HideAttackMoveDescription();
        }
        else
        {
            AddToBattleLog("Catch failed.");
            SkipPlayerTurn();
        }
    }

    public void SendNewSandwichToStorage()
    {
        CloseNewSandwichOptionsPanel();
        SandwichCollection.Instance.SendSandwichToStorage(enemy);
        EndBattle();
    }

    public void SendNewSandwichToTeam()
    {
        PlayerSandwichTeam.Instance.ShowAddNewSandwichToTeam();
    }

    private void CloseNewSandwichOptionsPanel()
    {
        newSandwichOptionsPanel.SetActive(false);
    }

    public void SwapActiveSandwich()
    {
        PlayerSandwichTeam.Instance.ShowPlayerTeamSwap();
        HideAttackMoveDescription();
    }

    public void HealPlayer()
    {
        if (Inventory.Instance.HasHealthPotion())
        {
            Inventory.Instance.UseHealthPotion();
            player.HealSandwich();
            UpdateUI();
            SkipPlayerTurn();
            AddToBattleLog("Player Healed");
        }
        else
        {
            AddToBattleLog("No Health Potions");
        }
    }

    public void SkipPlayerTurn()
    {
        isPlayerTurn = true;
        playerAttackButtonsPanelBlocker.SetActive(true);
        StartCoroutine(TimeBetweenTurns());
    }

    public void FleeBattle()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            EndBattle();
        }
        else
        {
            AddToBattleLog("Flee battle failed!");
            SkipPlayerTurn();
        }
    }

    private void AddToBattleLog(string text)
    {
        battleLogScrollRect.verticalNormalizedPosition = 1f;
        GameObject battleLog = Instantiate(battleLogPrefab, battleLogContainer);
        battleLog.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void UpdateUI()
    {
        if (!isFighting) return;
        playerSandwichImage.sprite = player.GetSprite();
        enemySandwichImage.sprite = enemy.GetSprite();
        UpdateInfoPanelsUI();
        UpdateCatchChanceUI();
    }

    private void UpdateCatchChanceUI()
    {
        float chance = 100 - enemy.GetHealthPercentage();
        if (chance < 20)
        {
            catchChance.text = "0 %";
        }
        else
        {
            catchChance.text = chance - 20 + " %";
        }
    }

    private void UpdateInfoPanelsUI()
    {
        enemyHealth.text = "Health: " + enemy.GetHealth() + "/" + enemy.GetMaxHealth();
        playerHealth.text = "Health: " + player.GetHealth() + "/" + player.GetMaxHealth();
        enemyRarity.text = enemy.GetSandwichType() + ": " + enemy.GetRarity();
        playerRarity.text = player.GetSandwichType() + ": " + player.GetRarity();
        playerName.text = player.GetMonsterName();
        enemyName.text = enemy.GetMonsterName();
    }
    
    public void StartBattle(SandwichMonster enemyMonster)
    {
        player = PlayerSandwichTeam.Instance.GetFirstLiveSandwich();
        if (player == null)
        {
            EndBattle();
            return;
        }
        ClearBattleLog();
        fightScreen.SetActive(true);
        isFighting = true;
        enemy = enemyMonster;
        SandwichMonsterManager.Instance.AddEncounter(enemy.GetMonsterName());
        AddToBattleLog("Encountered a wild " + enemy.GetMonsterName());
        UpdateUI();
        PlayerTurn();
        BuildPlayerAttackButtons();
        GameManager.Instance.DisablePlayerMovement();
        UIManager.Instance.HideGameHUD();
    }

    private void ClearBattleLog()
    {
        foreach (Transform child in battleLogContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void EndBattle()
    {
        isFighting = false;
        fightScreen.SetActive(false);
        enemy = null;
        HideAttackMoveDescription();
        CloseNewSandwichOptionsPanel();
        GameManager.Instance.EnablePlayerMovement();
        UIManager.Instance.ShowGameHUD();
    }

    private void ReduceEffects()
    {
        ReduceEffects(enemy);
        ReduceEffects(player);
        UpdateInflictedEffectsUI();
    }

    private void ReduceEffects(SandwichMonster sandwich)
    {
        if (enemy == null || player == null) return;
        
        List<AttackEffect> effectsToRemove = new List<AttackEffect>();
        foreach (AttackEffect effect in sandwich.GetInflictedEffects())
        {
            if (effect == null)
            {
                effectsToRemove.Add(effect);
                continue;
            }
            effect.ReduceEffectDuration();
            
            if (effect.GetEffectType() == AttackEffectType.Poison)
            {
                sandwich.TakeDamage(poisonDamageAmount);
                if (sandwich == player)
                {
                    AddToBattleLog("You are poisoned!");
                }
                else
                {
                    AddToBattleLog(enemy.GetMonsterName() + " is poisoned!");
                }
            }
            
            if (effect.GetEffectDuration() <= 0)
            {
                effectsToRemove.Add(effect);
            }
        }

        foreach (AttackEffect effect in effectsToRemove)
        {
            sandwich.RemoveEffect(effect);
        }
    }

    private void EnemyTurn()
    {
        isPlayerTurn = false;
        CheckHitChance(player, enemy.Attack());
        StartCoroutine(TimeBetweenTurns());
    }

    private void PlayerTurn()
    {
        if (player.HasEffect(AttackEffectType.Stun))
        {
            AddToBattleLog("You are stunned!");
            SkipPlayerTurn();
        }
        else
        {
            isPlayerTurn = true;
            playerAttackButtonsPanelBlocker.SetActive(false);
        }
        
        ReduceEffects();
    }

    private void SelectPlayerAttack(SandwichMonsterAttackMove attackMove)
    {
        playerAttackButtonsPanelBlocker.SetActive(true);
        CheckHitChance(enemy, attackMove);
        StartCoroutine(TimeBetweenTurns());
    }

    private void CheckHitChance(SandwichMonster monster, SandwichMonsterAttackMove attack)
    {
        
        if (enemy == null)
        {
            return;
        }
        
        if (attack == null)
        {
            if (!isPlayerTurn)
            {
                AddToBattleLog(enemy.GetMonsterName() + " is Stunned!");
            }
            return;
        }
        
        int random = Random.Range(0, 100);
        if (random <= attack.GetHitChance())
        {
            attack.InflictEffect(monster);
            UpdateInflictedEffectsUI();
            monster.TakeDamage(attack.GetDamage());

            if (!isPlayerTurn)
            {
                AddToBattleLog(enemy.GetMonsterName() + " used " + attack.GetMoveName() + "!");
            }
            else
            {
                AddToBattleLog(attack.GetMoveName() + " Attack Successful!");
            }
        }
        else
        {
            if (!isPlayerTurn)
            {
                AddToBattleLog(enemy.GetMonsterName() + " missed!");
            }
            else
            {
                AddToBattleLog(attack.GetMoveName() + " missed!");
            }
        }
    }

    private IEnumerator TimeBetweenTurns()
    {
        UpdateUI();
        yield return new WaitForSeconds(timeBetweenTurns);
        if (isFighting)
        {
            if (isPlayerTurn)
            {
                EnemyTurn();
            }
            else
            {
                PlayerTurn();
            }

            UpdateUI();
        }

    }

    private void BuildPlayerAttackButtons()
    {
        foreach (Transform child in playerAttackButtonsContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (SandwichMonsterAttackMove attack in player.GetAttacks())
        {
            GameObject choice = Instantiate(attackButtonPrefab, playerAttackButtonsContent.transform);
            choice.GetComponentInChildren<TextMeshProUGUI>().text = attack.GetMoveName();
            Button choiceButton = choice.GetComponentInChildren<Button>();
            choiceButton.onClick.AddListener(() =>
            {
                SelectPlayerAttack(attack);
            });
            choice.GetComponent<ShowAttackMoveDescription>().SetAttack(attack);
        }
    }
    
    */
}