using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    
    private int[] respawnTimes;
    private readonly string ENEMY_SPAWNER = "ENEMY_SPAWNER";

    private void Awake()
    {
        respawnTimes = new int[enemies.Length];
    }

    private void Start()
    {
        SetUpdateFrameNumber();
        if (!SceneChangeManager.Instance.HasVisitedScene(SceneManager.GetActiveScene()))
        {
            ResetRespawnTimes();
        }
        else
        {
            LoadRespawnTimes();
        }
        ActivateEnemies();
    }
    
    public static void SpawnEnemiesInRoom(BSPNode roomNode, GameObject[] enemyPrefabs, int minEnemies = 1, int maxEnemies = 3, int padding = 1)
    {
        if (!roomNode.Room.HasValue || enemyPrefabs.Length == 0)
            return;

        RectInt room = roomNode.Room.Value;

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            // Random position inside room, respecting padding
            float x = Random.Range(room.x + padding, room.x + room.width - padding);
            float y = Random.Range(room.y + padding, room.y + room.height - padding);

            Vector3 spawnPos = new Vector3(x, y, 0f); // 2D setup, Z = 0

            // Pick a random enemy prefab
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject.Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void ActivateEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (respawnTimes[i] <= GameManager.Instance.gamePlayTime)
            {
                enemies[i].SetActive(true);
            }
            else
            {
                enemies[i].SetActive(false);
            }
        }
    }

    private void SetUpdateFrameNumber()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<EnemyBrain>().updateFrameNumber = i;
        }
    }

    public void SetRespawnTime(EnemyHealth enemy, int respawnTime = Settings.defaultEnemyRespawnTime)
    {
        int index = GetEnemyIndex(enemy.gameObject);
        if (index == -1)
        {
            return;
        }
        respawnTimes[index] = GameManager.Instance.gamePlayTime + respawnTime;
    }

    private int GetEnemyIndex(GameObject enemyToFind)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == enemyToFind)
            {
                return i;
            }
        }
        
        return -1;
    }

    private void LoadRespawnTimes()
    {
        if (SaveGame.Exists(ENEMY_SPAWNER + SceneManager.GetActiveScene().name))
        {
            respawnTimes = SaveGame.Load<int[]>(ENEMY_SPAWNER + SceneManager.GetActiveScene().name);
        }
    }

    private void ResetRespawnTimes()
    {
        for (int i = 0; i < respawnTimes.Length; i++)
        {
            respawnTimes[i] = 0;
        }
    }

    public void SaveRespawnTimes()
    {
        SaveGame.Save(ENEMY_SPAWNER + SceneManager.GetActiveScene().name, respawnTimes);
    }

    private void OnDisable()
    {
        SaveRespawnTimes();
    }
}