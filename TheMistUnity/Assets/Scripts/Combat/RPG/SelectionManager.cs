using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : Singleton<SelectionManager>
{
    public static event Action<EnemyBrainRPG> OnEnemySelectedEvent;
    public static event Action OnNoSelectionEvent;
    
    [Header("Config")]
    [SerializeField] private LayerMask enemyMask;

    private Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        SelectEnemy();
    }

    private void SelectEnemy()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero, Mathf.Infinity, enemyMask);

            //using enemy mask so it will only detect collider of enemy
            if (hit.collider != null)
            {
                EnemyBrainRPG enemy = hit.collider.GetComponent<EnemyBrainRPG>();
                if (enemy == null)
                {
                    return;
                }

                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth.CurrentHealth <= 0f)
                {
                    EnemyLoot enemyLoot = enemy.GetComponent<EnemyLoot>();
                    LootManager.Instance.ShowLoot(enemyLoot);
                }
                else
                {
                    OnEnemySelectedEvent?.Invoke(enemy);
                }
                
            }
            else
            {
                OnNoSelectionEvent?.Invoke();
            }
        }
    }
    

}
