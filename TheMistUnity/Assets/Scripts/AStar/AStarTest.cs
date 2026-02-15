using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    [SerializeField] private AStarArea area;
    private Grid grid;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startGridPosition;
    private Vector3Int endGridPosition;
    private TileBase startPathTile;
    private TileBase finishPathTile;
    
    private Vector3Int noValue = new Vector3Int(9999,9999,9999);
    private Stack<Vector3> pathStack;

    private void Start()
    {
        startPathTile = GameManager.Instance.preferredEnemyPathTile;
        finishPathTile = GameManager.Instance.enemyUnwalkableCollisionTilesArray[0];
        
        pathStack = null;
        frontTilemap = area.frontTilemap;
        grid = area.grid;
        startGridPosition = noValue;
        endGridPosition = noValue;

        SetUpPathTilemap();
    }

    private void Update()
    {
        if(area == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null) return;

        
        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    private void DisplayPath()
    {
        if(startGridPosition == noValue || endGridPosition == noValue) return;
        
        pathStack = AStar.BuildPath(area, startGridPosition, endGridPosition);
        
        if(pathStack == null) return;

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
        }
    }

    private void ClearPath()
    {
        if(pathStack == null) return;

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    private void SetStartPosition()
    {
        if (startGridPosition == noValue)
        {
            startGridPosition = grid.WorldToCell(HelperMethods.GetMouseWorldPosition());
            
            if (!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }
            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }
    
    private void SetEndPosition()
    {
        if (endGridPosition == noValue)
        {
            endGridPosition = grid.WorldToCell(HelperMethods.GetMouseWorldPosition());
            
            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }
            pathTilemap.SetTile(endGridPosition, finishPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }

    private bool IsPositionWithinBounds(Vector3Int position)
    {
        if (position.x < area.gridLowerBounds.x || position.x > area.gridUpperBounds.x ||
            position.y < area.gridLowerBounds.y || position.y > area.gridUpperBounds.y)
        {
            return false;
        }
        
        return true;
    }

    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = grid.transform.Find("Grid/Front(Clone)");

        if (tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.gameObject.tag = "Untagged";
        }
        else
        {
            pathTilemap = grid.transform.Find("Front(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

}
