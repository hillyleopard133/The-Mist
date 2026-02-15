using Unity.VisualScripting;
using UnityEngine;

public class Player : Singleton<Player>
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    
    [Header("Test")]
    public ItemHealthPotion HealthPotion;
    public ItemManaPotion ManaPotion;
    
    public PlayerStats Stats => stats;
    public PlayerMana PlayerMana { get; private set; }
    public PlayerHealth PlayerHealth {get; private set;}

    public PlayerAttack PlayerAttack { get; private set; }

    private PlayerAnimations animations;

    protected override void Awake()
    {
        base.Awake();
        PlayerMana = GetComponent<PlayerMana>();
        PlayerHealth = GetComponent<PlayerHealth>();
        PlayerAttack = GetComponent<PlayerAttack>();
        animations = GetComponent<PlayerAnimations>();
    }

    public void SavePlayerStats()
    {
        stats.SavePlayerStats();
    }

    public void LoadPlayerStats()
    {
        stats.LoadPlayerStats();
    }

    public void ResetPlayer()
    {
        stats.ResetPlayer();
        animations.ResetPlayer();
        PlayerMana.ResetMana();
    }

    public void RespawnPlayer()
    {
        SceneChangeManager.Instance.LoadCheckpoint();
        PlayerMana.ResetMana();
        PlayerHealth.ResetHealth();
        animations.ResetPlayer();
    }
}
