using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Stack<Vector3> BuildPath(AStarArea area, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        GridNodes gridNodes = new GridNodes(area.gridUpperBounds.x - area.gridLowerBounds.x + 1, area.gridUpperBounds.y - area.gridLowerBounds.y + 1);
        
        Node startNode = gridNodes.GetGridNode(startGridPosition.x + Mathf.Abs(area.gridLowerBounds.x), startGridPosition.y + Mathf.Abs(area.gridLowerBounds.y));
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x + Mathf.Abs(area.gridLowerBounds.x), endGridPosition.y + Mathf.Abs(area.gridLowerBounds.y));
        
        Node endPathNode = FindShortestPath(area, startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, area);
        }
        return null;
    }

    private static Stack<Vector3> CreatePathStack(Node targetNode, AStarArea area)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();
        
        Node nextNode = targetNode;

        Grid grid = area.grid;
        if (grid == null) return null;
        
        Vector3 cellMidPoint = grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            Vector3 worldPosition = grid.CellToWorld(new Vector3Int(
                nextNode.gridPosition.x + area.gridLowerBounds.x,
                nextNode.gridPosition.y + area.gridLowerBounds.y, 0));

            worldPosition += cellMidPoint;
            movementPathStack.Push(worldPosition);
            nextNode = nextNode.parentNode;
        }
        
        return movementPathStack;
    }

    private static Node FindShortestPath(AStarArea area, Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, 
        HashSet<Node> closedNodeHashSet)
    {
        openNodeList.Add(startNode);

        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);
            
            if(currentNode == targetNode) return currentNode;
            
            closedNodeHashSet.Add(currentNode);
            
            EvaluateCurrentNodeNeighbours(area, currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet);
        }
        return null;
    }

    private static void EvaluateCurrentNodeNeighbours(AStarArea area, Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if(i == 0 && j == 0) continue;
                
                validNeighbourNode = GetValidNodeNeighbour(area, currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, 
                    closedNodeHashSet);

                if (validNeighbourNode != null)
                {
                    int movementPenaltyForGridSpace = area.aStarMovementPenalty[
                        validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];
                    
                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;
                    
                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
        
        if(dstX > dstY) return (14 * dstY) + 10 * (dstX - dstY);
        return (14 * dstX) + 10 * (dstY - dstX);
    }

    private static Node GetValidNodeNeighbour(AStarArea area, int neighbourNodeXPosition, int neighbourNodeYPosition,
        GridNodes gridNodes, HashSet<Node> closedNodeHashSet)
    {
        if (neighbourNodeXPosition >= area.gridUpperBounds.x - area.gridLowerBounds.x || neighbourNodeXPosition < 0 || 
            neighbourNodeYPosition >= area.gridUpperBounds.y - area.gridLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }
        
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        int movementPenaltyForGridSpace = area.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        if (closedNodeHashSet.Contains(neighbourNode) || movementPenaltyForGridSpace == 0) return null;
        
        return neighbourNode;
    }
    
}
