using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public event Action OnEnemyDeadEvent;
    
    [Header("Config")]
    public float health;
    
    [HideInInspector] public float CurrentHealth { get; private set; }
    
    private Rigidbody2D rb2D;
    private EnemyBrainRPG _enemyBrainRpg;
    private EnemyLoot enemyLoot;
    private EnemySelector enemySelector;
    private EnemyAnimations enemyAnimations;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        _enemyBrainRpg = GetComponent<EnemyBrainRPG>();
        enemyLoot = GetComponent<EnemyLoot>();
        enemySelector = GetComponent<EnemySelector>();
        enemyAnimations = GetComponent<EnemyAnimations>();
    }

    private void Start()
    {
        CurrentHealth = health;
        _enemyBrainRpg.isAlive = true;
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }
    
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        //UIManager.Instance.EnemyInInfoPanelDamaged(enemyBrain);
        if (CurrentHealth <= 0)
        {
            DisableEnemy();
            QuestManager.Instance.AddProgress("Kill2Enemy", 1);
            QuestManager.Instance.AddProgress("Kill5Enemy", 1);
            QuestManager.Instance.AddProgress("Kill10Enemy", 1);
            SetEnemyRespawnTime();
        }
        else
        {
            AudioManager.Instance.PlayEnemyDamageSound();
            //DamageManager.Instance.ShowDamageText(amount, transform);
        }
    }

    private void SetEnemyRespawnTime()
    {
        FindFirstObjectByType<EnemySpawner>().SetRespawnTime(this, 30);
    }

    private void DisableEnemy()
    {
        AudioManager.Instance.PlayEnemyDeathSound();
        enemyAnimations.SetDeadAnimation();
        _enemyBrainRpg.enabled = false;
        _enemyBrainRpg.isAlive = false;
        enemySelector.NoSelectionCallback();
        rb2D.bodyType = RigidbodyType2D.Static;
        OnEnemyDeadEvent?.Invoke();
        //GameManager.Instance.AddPlayerExp(enemyLoot.ExpDrop);
    }
}
