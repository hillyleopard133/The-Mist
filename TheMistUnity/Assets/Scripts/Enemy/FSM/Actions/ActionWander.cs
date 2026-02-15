using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWander : FSMAction
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float wanderTime;
    [SerializeField] private Vector2 moveRange;
    
    private Stack<Vector3> movementSteps;
    public Coroutine moveEnemyRoutine;
    private WaitForFixedUpdate waitForFixedUpdate;

    private int updateFrameNumber;
    private Vector3 movePosition;
    private float timer;
    
    private EnemyBrain enemyBrain;
    private AStarArea area;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
    }

    private void Start()
    {
        area = FindObjectOfType<AStarArea>();
        updateFrameNumber = enemyBrain.updateFrameNumber;
    }
    
    public override void Act()
    {
        if(!enemyBrain.isAlive) return;
        MoveEnemy();
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
    
    private void MoveEnemy()
    {
        timer -= Time.deltaTime;

        if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber) return;

        if (timer <= 0f || Vector3.Distance(transform.position, movePosition) <= 0.5f)
        {
            timer = wanderTime;
            CreatePath();

            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    StopCoroutine(moveEnemyRoutine);
                }

                //TODO wait at destination for a second before going to next
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

            while (Vector3.Distance(nextPosition, transform.position) > 0.1f)
            {
                MoveRigidBody(nextPosition, speed);
                
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

        movementSteps = AStar.BuildPath(area, enemyGridPosition, GetNewDestination());

        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
    }

    private Vector3Int GetNewDestination()
    {
        Vector3Int destinationGrid = Vector3Int.zero;
        int obstacle = 0;
        int attempts = 0;
        while (obstacle != 1 && attempts < 100)
        {
            attempts++;
            float randomX = Random.Range(-moveRange.x, moveRange.x);
            float randomY = Random.Range(-moveRange.y, moveRange.y);
            movePosition = transform.position + new Vector3(randomX, randomY);
            
            destinationGrid = area.grid.WorldToCell(movePosition);
            Vector3Int destinationPosition = destinationGrid - new Vector3Int(area.gridLowerBounds.x, area.gridLowerBounds.y);

            try
            {
                obstacle = area.aStarMovementPenalty[destinationPosition.x, destinationPosition.y];
            }catch{}
        }
        return destinationGrid;
    }

    private void OnDrawGizmosSelected()
    {
        if (moveRange != Vector2.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, moveRange * 2f);
            Gizmos.DrawLine(transform.position, movePosition);
        }
    }
    
}
