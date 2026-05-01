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
    private CombatManager combatManager;
    private TempleManager templeManager;

    private bool gameIsActive;
    
    //public Vector3 playerPosition;
    private Vector3 startScreenPosition = new Vector3(0, -0.5f, 0);
    
    [SerializeField] private string startingCheckpoint;
    [SerializeField] public bool isFirstTimeStartingGame;
    
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
        combatManager = CombatManager.Instance;
        templeManager = TempleManager.Instance;
        
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
        combatManager.SaveCombatData();
        templeManager.SaveTemples();
    }

    //Reset game data here
    public void StartNewGame()
    {
        uIManager.HideNewGameWarning();
        pauseGameManager.UnPause();
        StartCoroutine(LoadNewGameSceneCoroutine());
        ActivateGame();
        ResetGameData();
        //TODO AudioManager.Instance.NewGameMusic();
        SetCheckpoint(startingCheckpoint);
        uIManager.EnableLoadButton();
        SaveGameData();
        SaveGame.Save(FIRST_START, false);
    }

    private void ResetGameData()
    {
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
        combatManager.ResetCombatData();
        templeManager.ResetTemples();
    }
    
    private IEnumerator LoadNewGameSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        
        UIManager.Instance.ActivateLoadingScreen(true);
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            UIManager.Instance.UpdateLoadingProgress(progress);
            yield return null;
        }
        UIManager.Instance.ActivateLoadingScreen(false);
        
        SaveGameData();
    }

    //Load game data here
    public void LoadSaveGame()
    {
        if (SaveGame.Exists(GAME_LOCATION))
        {
            StartCoroutine(LoadSaveGameCoroutine());
        }
    }
    
    private IEnumerator LoadSaveGameCoroutine()
    {
        SceneData data = SaveGame.Load<SceneData>(GAME_LOCATION);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.sceneName);
        
        UIManager.Instance.ActivateLoadingScreen(true);
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            UIManager.Instance.UpdateLoadingProgress(progress);
            yield return null;
        }
        
        yield return null;
        
        audioManager.LoadCurrentMusic();
        Vector3 newPosition = new Vector3(data.playerPosX, data.playerPosY, 0);
        pauseGameManager.UnPause();
        ActivateGame();
        player.gameObject.transform.position = newPosition;
        inventory.LoadInventory();
        skillsManager.LoadSkills();
        equipmentManager.LoadEquipment();
        questManager.LoadQuestData();
        coinManager.LoadCoins();
        combatManager.LoadCombatData();
        templeManager.LoadTemples();
        dialogueManager.GetDialogueQuestManager().LoadDialogueTriggers();
        if (npcFollowerManager.gameObject.transform.childCount == 0)
        {
            npcFollowerManager.InstantiateAppropriateNPCPrefabs();
        }
        
        UIManager.Instance.UpdateLoadingProgress(1f);
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.ActivateLoadingScreen(false);
        dialogueManager.SelectNPC(null);
    }
}