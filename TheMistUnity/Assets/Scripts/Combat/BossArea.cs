using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArea : EnemyArea
{
    public override void StartCombat()
    {
        if(hasEntered) return;
        combatManager.SetIsBossFight(true);
        combatManager.EnterCombat(enemies, gridType);
        hasEntered = true;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public override void SpawnEnemies()
    {
        foreach (EnemyDetails enemy in enemies)
        {
            GameObject enemyObject = Instantiate(enemy.enemyPrefab, spawnArea.bounds.center, Quaternion.identity, transform);
            enemy.enemyCombatBrain = enemyObject.GetComponent<EnemyCombatBrain>();
            enemy.enemyCombatBrain.enemyDetails = enemy;
            EnemyBrain brain = enemyObject.GetComponent<EnemyBrain>();
            if (brain != null) brain.area = aStarArea;
        }
    }
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            combatManager.SetEnemyArea(this);
        }
    }
}
