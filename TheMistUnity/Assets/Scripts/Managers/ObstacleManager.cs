using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleManager: Singleton<ObstacleManager>
{
    private List<string> obstacles = new List<string>();

    private const string OBSTACLES = "OBSTACLES";

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (string obstacleName in obstacles)
        {
            GameObject obstacle = GameObject.Find(obstacleName);
            if (obstacle != null)
            {
                obstacle.SetActive(false);
            }
        }
    }

    public void AddObstacle(string obstacleName)
    {
        obstacles.Add(obstacleName);
        GameObject obstacle = GameObject.Find(obstacleName);
        obstacle.SetActive(false);
        SaveObstacles();
    }

    public void ResetObstacles()
    {
        obstacles.Clear();
        SaveObstacles();
    }

    public void LoadObstacles()
    {
        if (SaveGame.Exists(OBSTACLES))
        {
            obstacles = SaveGame.Load<List<string>>(OBSTACLES);
        }
    }

    public void SaveObstacles()
    {
        SaveGame.Save(OBSTACLES, obstacles);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}