using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEnemies : MonoBehaviour
{
    [SerializeField] private EnemyDetails[] enemies;
    [SerializeField] private EnemyDetails[] bosses;
    [SerializeField] private int minEnemies, maxEnemies;
    [SerializeField] private bool spawnOnLoad;
    private EnemyArea[] enemyAreas;
    private BossArea[] bossAreas;

    public static SceneEnemies Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(spawnOnLoad) InitiateEnemyAreas();
    }
    
    public void InitiateEnemyAreas()
    {
        enemyAreas = FindObjectsByType<EnemyArea>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (EnemyArea area in enemyAreas)
        {
            if (area is BossArea bossArea)
            {
                bossArea.ClearEnemies();
                bossArea.AddEnemy(bosses[0]);
                bossArea.SpawnEnemies();
            }
            else
            {
                area.ClearEnemies();
                FillEnemyArea(area);
                area.SpawnEnemies();
            }
        }
    }

    private void FillEnemyArea(EnemyArea area)
    {
        int randomNumber = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < randomNumber; i++)
        {
            int randomEnemy = Random.Range(0, enemies.Length);
            EnemyDetails enemy = enemies[randomEnemy].CopyEnemy();
            enemy.Index = i;
            area.AddEnemy(enemy);
        }
    }
}
