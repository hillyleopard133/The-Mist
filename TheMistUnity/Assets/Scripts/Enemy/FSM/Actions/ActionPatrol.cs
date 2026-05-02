using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPatrol : FSMAction
{
    [Header("Config")] 
    [SerializeField] private float speed;

    private Waypoint waypoint;
    private int pointIndex;
    private Vector3Int gridDestination;
    
    [HideInInspector] public Stack<Vector3> movementSteps;
    public Coroutine moveEnemyRoutine;
    private WaitForFixedUpdate waitForFixedUpdate;

    private int updateFrameNumber;
    private Vector3 movePosition;
    
    private EnemyBrain _enemyBrain;
    private AStarArea area;

    private void Awake()
    {
        waypoint = GetComponent<Waypoint>();
        _enemyBrain = GetComponent<EnemyBrain>();
    }

    private void Start()
    {
        area = FindFirstObjectByType<AStarArea>();
        updateFrameNumber = _enemyBrain.updateFrameNumber;
    }

    public override void Act()
    {
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
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber) return;

        if (Vector3.Distance(transform.position, gridDestination) <= 1f || movementSteps == null)
        {
            UpdateNextPosition();
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
        _enemyBrain.animations.SetMoveBoolTransition(true);  
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            while (Vector3.Distance(nextPosition, transform.position) > 0.05f)
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
        _enemyBrain.animations.SetMoveAnimation(direction);
        _enemyBrain.rb.MovePosition(_enemyBrain.rb.position + (direction * (moveSpeed * Time.fixedDeltaTime)));
    }
    
    private void CreatePath()
    {
        Grid grid = area.grid;
        if (grid == null) return;
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        movementSteps = AStar.BuildPath(area, enemyGridPosition, gridDestination);

        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
    }

    private void UpdateNextPosition()
    {
        pointIndex++;
        if (pointIndex > waypoint.Points.Length - 1)
        {
            pointIndex = 0;
        }
        
        gridDestination = area.grid.WorldToCell(GetCurrentPosition());
    }

    private Vector3 GetCurrentPosition()
    {
        return waypoint.GetPosition(pointIndex);
    }
    
}
