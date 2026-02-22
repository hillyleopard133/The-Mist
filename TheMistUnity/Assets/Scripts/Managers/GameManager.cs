using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
       [SerializeField] private Player player;
       
       [Header("Pathfinding")]
       public TileBase[] enemyUnwalkableCollisionTilesArray;
       public TileBase preferredEnemyPathTile;
       public Player Player => player;
       
       [HideInInspector] public int gamePlayTime;
       private float secondsTimer;
       
       private readonly string PLAY_TIME = "PLAY_TIME";

       private void Start()
       {
              LoadTimer();
       }
       
       private void Update()
       {
              if (SaveLoadManager.Instance.GameIsActive() && !PauseGameManager.Instance.isPaused)
              {
                     RunTimer();
              }
       }

       private void RunTimer()
       {
              secondsTimer += Time.deltaTime;

              if (secondsTimer >= 1f)
              {
                     secondsTimer -= 1f;
                     gamePlayTime++;
              }
       }

       private void LoadTimer()
       {
              if (SaveGame.Exists(PLAY_TIME))
              {
                     gamePlayTime = SaveGame.Load<int>(PLAY_TIME);
              }
       }

       public void SaveTimer()
       {
              SaveGame.Save(PLAY_TIME, gamePlayTime);
       }

       public void ResetTimer()
       {
              gamePlayTime = 0;
              SaveTimer();
       }

       /*
       public void AddPlayerExp(float expAmount)
       {
              PlayerExp playerExp = player.GetComponent<PlayerExp>();
              playerExp.AddExp(expAmount);
       }
       */

       public void DisablePlayerMovement()
       {
              player.GetComponent<PlayerMovement>().DisableMovement();
       }

       public void EnablePlayerMovement()
       {
              player.GetComponent<PlayerMovement>().EnableMovement();
       }
       
       
}
