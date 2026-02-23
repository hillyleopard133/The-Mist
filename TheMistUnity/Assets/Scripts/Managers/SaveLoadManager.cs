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
    private NPCFollowerManager npcFollowerManager;
    private GameManager gameManager;
    private CameraManager cameraManager;
    private PauseGameManager pauseGameManager;
    private EquipmentManager equipmentManager;
    private QuestManager questManager;
    private DialogueManager dialogueManager;
    private SceneChangeManager sceneChangeManager;
    private CoinManager coinManager;
    private AudioManager audioManager;
    private SkillsManager skillsManager;

    private bool gameIsActive;
    
    //public Vector3 playerPosition;
    private Vector3 startScreenPosition = new Vector3(0, -0.5f, 0);
    
    [SerializeField] private string startingCheckpoint;
    [SerializeField] private bool isFirstTimeStartingGame;
    
    private readonly string GAME_LOCATION = "MY_LOCATION";
    private readonly string CHECKPOINT = "CHECKPOINT";
    private readonly string FIRST_START = "FIRST_START";
    
    private void Start()
    {
        player = Player.Instance;
        uIManager = UIManager.Instance;
        inventory = Inventory.Instance;
        npcFollowerManager = NPCFollowerManager.Instance;
        gameManager = GameManager.Instance;
        cameraManager = CameraManager.Instance;
        pauseGameManager = PauseGameManager.Instance;
        equipmentManager = EquipmentManager.Instance;
        questManager = QuestManager.Instance;
        dialogueManager = DialogueManager.Instance;
        sceneChangeManager = SceneChangeManager.Instance;
        coinManager = CoinManager.Instance;
        audioManager = AudioManager.Instance;
        skillsManager = SkillsManager.Instance;
        
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
        npcFollowerManager.gameObject.SetActive(true);
        uIManager.ShowGameHUD();
        uIManager.HideStartMenu();
        gameManager.EnablePlayerMovement();
        cameraManager.SetCameraSize(6);
        gameIsActive = true;
    }

    private void DeactivateGame()
    {
        npcFollowerManager.gameObject.SetActive(false);
        uIManager.HideGameHUD();
        uIManager.ShowStartMenu();
        gameManager.DisablePlayerMovement();
        player.GetComponent<PlayerAnimations>().ResetPlayer();
        cameraManager.SetCameraSize(8);
        gameIsActive = false;
    }
    
    //Save Game data here
    public void QuitToMenu()
    {
        SaveGameData();
        SceneManager.LoadScene(0);
        DeactivateGame();
        player.gameObject.transform.position = startScreenPosition;
        pauseGameManager.UnPause();
        uIManager.CloseAllPanels();
    }

    //Save Game data here
    public void SaveGameData()
    {
        sceneChangeManager.SaveGameLocation();
        questManager.SaveQuestData();
        equipmentManager.SaveEquipment();
        inventory.SaveInventory();
        skillsManager.SaveSkills();
        gameManager.SaveTimer();
    }

    //Reset game data here
    public void StartNewGame()
    {
        uIManager.HideNewGameWarning();
        pauseGameManager.UnPause();
        StartCoroutine(LoadSceneCoroutine());
        ActivateGame();
        ResetGameData();
        //TODO AudioManager.Instance.NewGameMusic();
        SetCheckpoint(startingCheckpoint);
        uIManager.EnableLoadButton();
        SaveGame.Save(FIRST_START, false);
    }

    private void ResetGameData()
    {
        //player.ResetPlayer();
        inventory.ResetInventory();
        questManager.ResetQuests();
        equipmentManager.ResetEquipment();
        coinManager.ResetCoins();
        dialogueManager.GetDialogueQuestManager().ResetDialogueTriggers();
        dialogueManager.ResetNPCs();
        npcFollowerManager.ResetFollowing();
        gameManager.ResetTimer();
        sceneChangeManager.ResetVisitedScenes();
        uIManager.ResetPartyUnlocks();
        skillsManager.ResetSkills();
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
            audioManager.LoadCurrentMusic();
            Vector3 newPosition = new Vector3(data.playerPosX, data.playerPosY, 0);
            pauseGameManager.UnPause();
            ActivateGame();
            player.gameObject.transform.position = newPosition;
            //player.LoadPlayerStats();
            inventory.LoadInventory();
            skillsManager.LoadSkills();
            equipmentManager.LoadEquipment();
            questManager.LoadQuestData();
            coinManager.LoadCoins();
            dialogueManager.GetDialogueQuestManager().LoadDialogueTriggers();
            if (npcFollowerManager.gameObject.transform.childCount == 0)
            {
                npcFollowerManager.InstantiateAppropriateNPCPrefabs();
            }
        }
    }
}