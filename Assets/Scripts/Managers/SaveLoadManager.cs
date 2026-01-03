using System.Collections;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
       //Instances
    private Player player;
    private UIManager uIManager;
    private Inventory inventory;

    private bool gameIsActive;
    
    //public Vector3 playerPosition;
    private Vector3 startScreenPosition = new Vector3(0, -0.5f, 0);
    
    [SerializeField] private string startingCheckpoint;
    [SerializeField] private bool isFirstTimeStartingGame;
    
    private readonly string GAME_LOCATION = "MY_LOCATION";
    private readonly string CHECKPOINT = "CHECKPOINT";
    private readonly string FIRST_START = "FIRST_START";
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        player = Player.Instance;
        uIManager = UIManager.Instance;
        inventory = Inventory.Instance;
        DeactivateGame();    
        
        //TODO add this, run it and then remove it before building game!
        //SaveGame.Delete(FIRST_START);
        
        if (SaveGame.Exists(FIRST_START))
        {
            isFirstTimeStartingGame = SaveGame.Load<bool>(FIRST_START);
        }
        if (isFirstTimeStartingGame)
        {
            UIManager.Instance.DisableLoadButton();
        }
    }
    
    public void SetCheckpoint(string checkpoint)
    {
        SaveGame.Save(CHECKPOINT, checkpoint);
        SaveGameData();
    }

    public bool GameIsActive()
    {
        return gameIsActive;
    }

    private void ActivateGame()
    {
        NPCFollowerManager.Instance.gameObject.SetActive(true);
        uIManager.ShowGameHUD();
        uIManager.HideStartMenu();
        GameManager.Instance.EnablePlayerMovement();
        CameraManager.Instance.SetCameraSize(6);
        gameIsActive = true;
    }

    private void DeactivateGame()
    {
        NPCFollowerManager.Instance.gameObject.SetActive(false);
        uIManager.HideGameHUD();
        uIManager.ShowStartMenu();
        GameManager.Instance.DisablePlayerMovement();
        player.GetComponent<PlayerAnimations>().ResetPlayer();
        CameraManager.Instance.SetCameraSize(8);
        gameIsActive = false;
    }
    
    //Save Game data here
    public void QuitToMenu()
    {
        SaveGameData();
        SceneManager.LoadScene(0);
        DeactivateGame();
        player.gameObject.transform.position = startScreenPosition;
        PauseGameManager.Instance.UnPause();
        uIManager.CloseAllPanels();
    }

    //Save Game data here
    public void SaveGameData()
    {
        SceneChangeManager.Instance.SaveGameLocation();
        QuestManager.Instance.SaveQuestData();
        player.SavePlayerStats();
        inventory.SaveEquippedWeapon();
        GameManager.Instance.SaveTimer();
    }

    //Reset game data here
    public void StartNewGame()
    {
        UIManager.Instance.HideNewGameWarning();
        PauseGameManager.Instance.UnPause();
        StartCoroutine(LoadSceneCoroutine());
        ActivateGame();
        ResetGameData();
        //AudioManager.Instance.NewGameMusic();
        SetCheckpoint(startingCheckpoint);
        UIManager.Instance.EnableLoadButton();
        SaveGame.Save(FIRST_START, false);
    }

    private void ResetGameData()
    {
        player.ResetPlayer();
        inventory.ResetInventory();
        ResetInitialWeapon();
        QuestManager.Instance.ResetQuests();
        CoinManager.Instance.ResetCoins();
        DialogueManager.Instance.GetDialogueQuestManager().ResetDialogueTriggers();
        DialogueManager.Instance.ResetNPCs();
        NPCFollowerManager.Instance.ResetFollowing();
        GameManager.Instance.ResetTimer();
        SceneChangeManager.Instance.ResetVisitedScenes();
    }

    private void ResetInitialWeapon()
    {
        player.GetComponent<PlayerAttack>().ResetInitialWeapon();
        inventory.SaveEquippedWeapon();
        inventory.LoadEquippedWeapon();
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        SaveGameData();
    }

    //Load game data here
    public void LoadSaveGame()
    {
        if (SaveGame.Exists(GAME_LOCATION))
        {
            SceneData data = SaveGame.Load<SceneData>(GAME_LOCATION);
            SceneManager.LoadScene(data.sceneName);
            AudioManager.Instance.LoadCurrentMusic();
            Vector3 newPosition = new Vector3(data.playerPosX, data.playerPosY, 0);
            PauseGameManager.Instance.UnPause();
            ActivateGame();
            player.gameObject.transform.position = newPosition;
            player.LoadPlayerStats();
            inventory.LoadEquippedWeapon();
            QuestManager.Instance.LoadQuestData();
            CoinManager.Instance.LoadCoins();
            DialogueManager.Instance.GetDialogueQuestManager().LoadDialogueTriggers();
            if (NPCFollowerManager.Instance.gameObject.transform.childCount == 0)
            {
                NPCFollowerManager.Instance.InstantiateAppropriateNPCPrefabs();
            }
        }
    }
}