using System.Collections.Generic;
using UnityEngine;

public class BSPAlgorithm
{
    public static void Split(BSPNode node, int minSize, int maxDepth, int depth = 0)
    {
        if (depth >= maxDepth || node.Bounds.width < minSize * 2 || node.Bounds.height < minSize * 2) return;

        bool splitHorizontally = Random.value > 0.5f;

        if (node.Bounds.width > node.Bounds.height)
        {
            splitHorizontally = false;
        }
        else if (node.Bounds.height > node.Bounds.width)
        {
            splitHorizontally = true;
        }

        if (splitHorizontally)
        {
            int splitY = Random.Range(minSize, node.Bounds.height - minSize);

            node.Left = new BSPNode(new RectInt(
                node.Bounds.x,
                node.Bounds.y,
                node.Bounds.width,
                splitY
            ));

            node.Right = new BSPNode(new RectInt(
                node.Bounds.x,
                node.Bounds.y + splitY,
                node.Bounds.width,
                node.Bounds.height - splitY
            ));
        }
        else
        {
            int splitX = Random.Range(minSize, node.Bounds.width - minSize);

            node.Left = new BSPNode(new RectInt(
                node.Bounds.x,
                node.Bounds.y,
                splitX,
                node.Bounds.height
            ));

            node.Right = new BSPNode(new RectInt(
                node.Bounds.x + splitX,
                node.Bounds.y,
                node.Bounds.width - splitX,
                node.Bounds.height
            ));
        }

        Split(node.Left, minSize, maxDepth, depth + 1);
        Split(node.Right, minSize, maxDepth, depth + 1);
    }

    public static void CreateRooms(BSPNode node, int minRoomWidth, int maxRoomWidth, int minRoomHeight, int maxRoomHeight, int padding = 1)
    {
        if (!node.IsLeaf)
        {
            CreateRooms(node.Left, minRoomWidth, maxRoomWidth, minRoomHeight, maxRoomHeight, padding);
            CreateRooms(node.Right, minRoomWidth, maxRoomWidth, minRoomHeight, maxRoomHeight, padding);
            return;
        }

        int maxWidth = Mathf.Min(node.Bounds.width - padding * 2, maxRoomWidth);
        int maxHeight = Mathf.Min(node.Bounds.height - padding * 2, maxRoomHeight);

        if (maxWidth < minRoomWidth || maxHeight < minRoomHeight)
            return; 

        int roomWidth = Random.Range(minRoomWidth, maxWidth + 1);
        int roomHeight = Random.Range(minRoomHeight, maxHeight + 1);

        //int roomX = Random.Range(node.Bounds.x + padding, node.Bounds.xMax - roomWidth - padding + 1);
        //int roomY = Random.Range(node.Bounds.y + padding, node.Bounds.yMax - roomHeight - padding + 1);
        
        int roomX = node.Bounds.x + (node.Bounds.width - roomWidth) / 2;
        int roomY = node.Bounds.y + (node.Bounds.height - roomHeight) / 2;

        node.Room = new RectInt(roomX, roomY, roomWidth, roomHeight);
    }

    public static void CreateCorridors(BSPNode node, List<RectInt> corridors)
    {
        if (node.Left == null || node.Right == null) return;

        RectInt roomA = GetRoom(node.Left);
        RectInt roomB = GetRoom(node.Right);

        Vector2Int centerA = new Vector2Int(roomA.x + roomA.width / 2, roomA.y + roomA.height / 2);
        Vector2Int centerB = new Vector2Int(roomB.x + roomB.width / 2, roomB.y + roomB.height / 2);
        
        int corridorWidth = 3;
        int half = corridorWidth / 2;

        if (Random.value > 0.5f)
        {
            int xStart = Mathf.Min(centerA.x, centerB.x);
            int xEnd = Mathf.Max(centerA.x, centerB.x);
            int y = centerA.y - half;
            corridors.Add(new RectInt(xStart, y, xEnd - xStart + 1, corridorWidth));

            int yStart = Mathf.Min(centerA.y, centerB.y);
            int yEnd = Mathf.Max(centerA.y, centerB.y);
            int x = centerB.x - half;
            corridors.Add(new RectInt(x, yStart - half, corridorWidth, yEnd - yStart + 1 + half));
        }
        else
        {
            int yStart = Mathf.Min(centerA.y, centerB.y);
            int yEnd = Mathf.Max(centerA.y, centerB.y);
            int x = centerA.x - half;
            corridors.Add(new RectInt(x, yStart, corridorWidth, yEnd - yStart + 1));

            int xStart = Mathf.Min(centerA.x, centerB.x);
            int xEnd = Mathf.Max(centerA.x, centerB.x);
            int y = centerB.y - half;
            corridors.Add(new RectInt(xStart - half, y, xEnd - xStart + 1 + half, corridorWidth));
        }
        
        CreateCorridors(node.Left, corridors);
        CreateCorridors(node.Right, corridors);
    }

    private static RectInt GetRoom(BSPNode node)
    {
        if (node.Room.HasValue) return node.Room.Value;
        if (node.Left != null) return GetRoom(node.Left);

        return GetRoom(node.Right);
    }
}
