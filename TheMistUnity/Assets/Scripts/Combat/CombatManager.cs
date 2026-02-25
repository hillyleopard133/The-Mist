using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : Singleton<CombatManager>
{
    [SerializeField] private GameObject combatTilemap;
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

    private void Start()
    {
        cameraManager = CameraManager.Instance;
        skillsManager = SkillsManager.Instance;
        uIManager = UIManager.Instance;
        gameManager = GameManager.Instance;
        inventory = Inventory.Instance;
    }

    public void EnterCombat(List<EnemyDetails> enemies)
    {
        this.enemies = enemies;
        gameManager.DisablePlayerMovement();
        gameManager.Player.gameObject.SetActive(false);
        uIManager.ActivateCombatScreen(enemies);
        cameraManager.ToggleCombatCamera();
        combatTilemap.SetActive(true);
        isFighting = true;
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
        combatTilemap.SetActive(false);
        isFighting = false;
    }

    private void UseItems()
    {
        foreach (InventoryItem item in usedItems)
        {
            inventory.ConsumeItem(item.ID);
        }
        usedItems.Clear();
    }
}
