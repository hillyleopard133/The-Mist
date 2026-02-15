using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    private PlayerAnimations playerAnimations;

    private void Awake()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    public void TakeDamage(float amount)
    {
        if (stats.Health <= 0)
        {
            return;
        }
        AudioManager.Instance.PlayPlayerDamageSound();
        stats.Health -= amount;
        DamageManager.Instance.ShowDamageText(amount, transform);
        if(stats.Health <= 0f)
        {
            stats.Health = 0f;
            PlayerDeath();
        }
    }

    public void RestoreHealth(float amount)
    {
        stats.Health += amount;
        if (stats.Health > stats.MaxHealth)
        {
            stats.Health = stats.MaxHealth;
        }
    }

    public bool CanRestoreHealth()
    {
        return stats.Health > 0 && stats.Health < stats.MaxHealth;
    }

    private void PlayerDeath()
    {
        AudioManager.Instance.PlayPlayerDeathSound();
        playerAnimations.SetDeadAnimation();
        UIManager.Instance.ActivateDeathScreen();
    }

    public void ResetHealth()
    {
        stats.Health = stats.MaxHealth;
    }
    
}
