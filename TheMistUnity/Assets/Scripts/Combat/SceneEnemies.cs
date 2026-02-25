using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEnemies : MonoBehaviour
{
    [SerializeField] private EnemyDetails[] enemies;
    [SerializeField] private int minEnemies, maxEnemies;
    private EnemyArea[] enemyAreas;

    private void Start()
    {
        InitiateEnemyAreas();
    }

    private void InitiateEnemyAreas()
    {
        enemyAreas = FindObjectsOfType<EnemyArea>();
        foreach (EnemyArea area in enemyAreas)
        {
            area.ClearEnemies();
            FillEnemyArea(area);
        }
    }

    private void FillEnemyArea(EnemyArea area)
    {
        int randomNumber = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < randomNumber; i++)
        {
            int randomEnemy = Random.Range(0, enemies.Length);
            area.AddEnemy(enemies[randomEnemy].CopyEnemy());
        }
    }
}
