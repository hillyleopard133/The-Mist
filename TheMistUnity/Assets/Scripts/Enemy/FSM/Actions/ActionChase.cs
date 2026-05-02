using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActionChase : FSMAction
{
    [Header("Config")] 
    [SerializeField] private float chaseSpeed;
    private int updateFrameNumber;
    
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    public Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private EnemyBrain enemyBrain;
    private AStarArea area;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
    }

    private void Start()
    {
        area = enemyBrain.area;
        updateFrameNumber = enemyBrain.updateFrameNumber;
    }
    
    public override void Act()
    {
        ChasePlayer();
    }
    
    public void StopMoving()
    {
        movementSteps = null;
        if (moveEnemyRoutine != null)
        {
            StopCoroutine(moveEnemyRoutine);
            moveEnemyRoutine = null;
        }
    }

    private void ChasePlayer()
    {
        if (enemyBrain.Player == null) return;
        
        MoveEnemy();
    }
    
    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber) return;

        if (currentEnemyPathRebuildCooldown <= 0f ||
            (Vector3.Distance(playerReferencePosition, GameManager.Instance.Player.transform.position) > Settings.playerMoveDistanceToRebuildPath))
        {
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;
            playerReferencePosition = GameManager.Instance.Player.transform.position;
            CreatePath();

            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }
    
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        enemyBrain.animations.SetMoveBoolTransition(true);
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();
            Vector2 direction = (nextPosition - transform.position).normalized;
            enemyBrain.animations.SetMoveAnimation(direction);

            while (Vector3.Distance(nextPosition, transform.position) > 1f)
            {
                MoveRigidBody(nextPosition, chaseSpeed);
                
                yield return waitForFixedUpdate;
            }
            yield return waitForFixedUpdate;
        }
    }
    
    private void MoveRigidBody(Vector3 destination, float moveSpeed)
    {
        Vector2 direction = (destination - transform.position).normalized;
        enemyBrain.animations.SetMoveAnimation(direction);
        enemyBrain.rb.MovePosition(enemyBrain.rb.position + (direction * (moveSpeed * Time.fixedDeltaTime)));
    }
    
    private void CreatePath()
    {
        Grid grid = area.grid;
        if (grid == null) return;
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(grid);

        movementSteps = AStar.BuildPath(area, enemyGridPosition, playerGridPosition);

        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
    }
    
    private Vector3Int GetNearestNonObstaclePlayerPosition(Grid grid)
    {
        Vector3 playerPosition = GameManager.Instance.Player.transform.position;
        Vector3Int playerCellPosition = grid.WorldToCell(playerPosition);
        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - area.gridLowerBounds.x, playerCellPosition.y - area.gridLowerBounds.y);
        
        int obstacle = Mathf.Min(area.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y]);

        if (obstacle != 0) return playerCellPosition;
        
        surroundingPositionList.Clear();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                surroundingPositionList.Add(new Vector2Int(i, j));
            }
        }

        for (int i = 0; i < 8; i++)
        {
            int index = Random.Range(0, surroundingPositionList.Count);
            
            try
            {
                obstacle = Mathf.Min(area.aStarMovementPenalty[adjustedPlayerCellPosition.x + surroundingPositionList[index].x, adjustedPlayerCellPosition.y + surroundingPositionList[index].y]);
                if (obstacle != 0)
                {
                    return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                }
            }
            catch
            {
            }
            surroundingPositionList.RemoveAt(index);
        }
        return playerCellPosition;
    }
}
