using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BayatGames.SaveGameFree;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private string currentScene;
    
    private readonly string GAME_LOCATION = "MY_LOCATION";
    private readonly string CHECKPOINT = "CHECKPOINT";
    private readonly string SCENES_VISITED = "SCENES_VISITED";
    
    private List<string> scenesVisited = new List<string>();
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        LoadScenesVisited();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SaveGameLocation()
    {
        SceneData sceneData = new SceneData();
        currentScene = SceneManager.GetActiveScene().name;
        sceneData.sceneName = currentScene;
        sceneData.playerPosX = Player.Instance.gameObject.transform.position.x;
        sceneData.playerPosY = Player.Instance.gameObject.transform.position.y;
        SaveGame.Save(GAME_LOCATION, sceneData);
        if(!scenesVisited.Contains(currentScene)) scenesVisited.Add(currentScene);
        SaveScenesVisited();
    }

    public bool HasVisitedScene(Scene scene)
    {
        return scenesVisited.Contains(scene.name);
    }

    private void LoadScenesVisited()
    {
        if (SaveGame.Exists(SCENES_VISITED))
        {
            scenesVisited = SaveGame.Load<List<string>>(SCENES_VISITED);
        }
    }

    private void SaveScenesVisited()
    {
        SaveGame.Save(SCENES_VISITED, scenesVisited);
    }

    public void ResetVisitedScenes()
    {
        scenesVisited.Clear();
        SaveScenesVisited();
    }
    
    public void LoadScene(string sceneName, string spawnLocation)
    {
        // Start loading the scene asynchronously and show the loading screen
        StartCoroutine(LoadSceneCoroutine(sceneName, spawnLocation));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, string spawnLocation)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            //TODO create loading screen
            //float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            //Debug.Log("Loading Progress: " + (progress * 100) + "%");

            yield return null;
        }
        
        SaveGame.Save(CHECKPOINT, spawnLocation);
        PositionPlayer(spawnLocation);
        SaveGameLocation();
        GameManager.Instance.SaveTimer();
    }

    public void LoadCheckpoint()
    {
        if (SaveGame.Exists(CHECKPOINT))
        {
            PositionPlayer(SaveGame.Load<string>(CHECKPOINT));
            UIManager.Instance.CloseAllPanels();
            PauseGameManager.Instance.UnPause();
        }
    }

    private void PositionPlayer(string spawnLocation)
    {
        GameObject spawnPoint = GameObject.Find(spawnLocation);

        if (spawnLocation != null)
        {
            Vector3 newPosition = spawnPoint.transform.position;
            newPosition.z = 0f;
            Player.Instance.gameObject.transform.position = newPosition;
        }
        
        MoveFollowingNPCsToPlayer();
    }

    private void MoveFollowingNPCsToPlayer()
    {
        GameObject npcParent = NPCFollowerManager.Instance.gameObject;
        
        foreach (Transform npc in npcParent.transform) 
        {
            npc.position = Player.Instance.transform.position;
            Rigidbody2D rb = npc.GetComponent<Rigidbody2D>(); 
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

}
