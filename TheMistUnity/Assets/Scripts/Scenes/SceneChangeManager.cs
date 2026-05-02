using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BayatGames.SaveGameFree;
using Unity.VisualScripting;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private string currentScene;
    
    private readonly string GAME_LOCATION = "MY_LOCATION";
    private readonly string CHECKPOINT = "CHECKPOINT";
    private readonly string SCENES_VISITED = "SCENES_VISITED";
    
    private List<string> scenesVisited = new List<string>();

    private Player player;
    private UIManager uIManager;
    private NPCFollowerManager npcFollowerManager;
    private PauseGameManager pauseGameManager;
    private GameManager gameManager;
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        LoadScenesVisited();
        player = Player.Instance;
        uIManager = UIManager.Instance;
        npcFollowerManager = NPCFollowerManager.Instance;
        pauseGameManager = PauseGameManager.Instance;
        gameManager = GameManager.Instance;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SaveGameLocation()
    {
        SceneData sceneData = new SceneData();
        sceneData.sceneName = currentScene;
        sceneData.playerPosX = player.gameObject.transform.position.x;
        sceneData.playerPosY = player.gameObject.transform.position.y;
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
        if (sceneName == "HamsterHoles")
        {
            player.gameObject.SetActive(false);
            npcFollowerManager.gameObject.SetActive(false);
            uIManager.HideGameHUD();
        }
        else
        {
            player.gameObject.SetActive(true);
            npcFollowerManager.gameObject.SetActive(true);
            uIManager.ShowGameHUD();
        }

        if (sceneName == "HamsterHoles" || currentScene == "HamsterHoles")
        {
            //CameraManager.Instance.ToggleHamsterCamera();
        }
        
        StartCoroutine(LoadSceneCoroutine(sceneName, spawnLocation));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, string spawnLocation)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        
        uIManager.ActivateLoadingScreen(true);
        
        float minimumDisplayTime = 0.3f;
        float timer = 0f;
        
        while (!asyncOperation.isDone || timer < minimumDisplayTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            uIManager.UpdateLoadingProgress(progress);
            yield return null;
        }
        yield return null;
        
        SaveGame.Save(CHECKPOINT, spawnLocation);
        currentScene = SceneManager.GetActiveScene().name;
        PositionPlayer(spawnLocation);
        SaveGameLocation();
        gameManager.SaveTimer();
        
        uIManager.ActivateLoadingScreen(false);
    }

    public void LoadCheckpoint()
    {
        if (SaveGame.Exists(CHECKPOINT))
        {
            PositionPlayer(SaveGame.Load<string>(CHECKPOINT));
            uIManager.CloseAllPanels();
            pauseGameManager.UnPause();
        }
    }

    private void PositionPlayer(string spawnLocation)
    {
        GameObject spawnPoint = GameObject.Find(spawnLocation);

        if (spawnPoint != null)
        {
            Vector3 newPosition = spawnPoint.transform.position;
            newPosition.z = 0f;
            player.gameObject.transform.position = newPosition;

            if (currentScene == "HamsterHoles")
            {
                GameObject.Find("Hamster").transform.position = newPosition;
            }
        }

        if (currentScene != "HamsterHoles")
        {
            MoveFollowingNPCsToPlayer();
        }
    }

    private void MoveFollowingNPCsToPlayer()
    {
        GameObject npcParent = npcFollowerManager.gameObject;
        
        foreach (Transform npc in npcParent.transform) 
        {
            npc.position = player.transform.position;
            Rigidbody2D rb = npc.GetComponent<Rigidbody2D>(); 
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

}
