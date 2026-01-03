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
    
    public static List<BSPNode> GetLeafNodes(BSPNode root)
    {
        List<BSPNode> leaves = new List<BSPNode>();
        TraverseLeaves(root, leaves);
        return leaves;
    }

    private static void TraverseLeaves(BSPNode node, List<BSPNode> leaves)
    {
        if (node == null) return;
        if (node.IsLeaf)
        {
            leaves.Add(node);
            return;
        }

        TraverseLeaves(node.Left, leaves);
        TraverseLeaves(node.Right, leaves);
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

    public static void CreateCorridors(BSPNode node, List<RectInt> corridors, int corridorWidth = 3)
    {
        if (node.Left == null || node.Right == null) return;

        RectInt roomA = GetRoom(node.Left);
        RectInt roomB = GetRoom(node.Right);

        CreateCorridor(roomA, roomB, corridors, corridorWidth);

        CreateCorridors(node.Left, corridors, corridorWidth);
        CreateCorridors(node.Right, corridors, corridorWidth);
    }
    
    public static void CreateCorridorsBetter(BSPNode node, List<RectInt> corridors, int corridorWidth)
    {
        if (node == null || node.IsLeaf) return;

        List<BSPNode> leftLeaves = GetLeafNodes(node.Left);
        List<BSPNode> rightLeaves = GetLeafNodes(node.Right);

        
        if (leftLeaves.Count > 0 && rightLeaves.Count > 0)
        {
            // Find nearest pair between left and right
            BSPNode nearestLeft = null;
            BSPNode nearestRight = null;
            float minDist = float.MaxValue;

            foreach (var l in leftLeaves)
            {
                Vector2Int lCenter = GetRoomCenter(l);
                foreach (var r in rightLeaves)
                {
                    Vector2Int rCenter = GetRoomCenter(r);
                    float dist = Vector2Int.Distance(lCenter, rCenter);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestLeft = l;
                        nearestRight = r;
                    }
                }
            }

            // Connect the closest pair
            if (nearestLeft != null && nearestRight != null)
            {
                CreateCorridor(nearestLeft.Room.Value, nearestRight.Room.Value, corridors, corridorWidth);
            }
        }

        // Recurse into children
        CreateCorridorsBetter(node.Left, corridors, corridorWidth);
        CreateCorridorsBetter(node.Right, corridors, corridorWidth);
    }


    public static void CreateCorridor(RectInt roomA, RectInt roomB, List<RectInt> corridors, int corridorWidth)
    {
        Vector2Int centerA = new Vector2Int(roomA.x + roomA.width / 2, roomA.y + roomA.height / 2);
        Vector2Int centerB = new Vector2Int(roomB.x + roomB.width / 2, roomB.y + roomB.height / 2);

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
            int verticalHeight = yEnd - yStart + 1 + corridorWidth - 1;
            corridors.Add(new RectInt(x, yStart - half, corridorWidth, verticalHeight));
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
            int horizontalWidth = xEnd - xStart + 1 + corridorWidth - 1;
            corridors.Add(new RectInt(xStart - half, y, horizontalWidth, corridorWidth));
        }
    }

    private static RectInt GetRoom(BSPNode node)
    {
        if (node.Room.HasValue) return node.Room.Value;
        if (node.Left != null) return GetRoom(node.Left);

        return GetRoom(node.Right);
    }
    
    public static List<BSPNode> CreateGridNodes(RectInt mapBounds, int rows, int cols)
    {
        List<BSPNode> nodes = new List<BSPNode>();
        int cellWidth = mapBounds.width / cols;
        int cellHeight = mapBounds.height / rows;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                RectInt cell = new RectInt(
                    mapBounds.x + x * cellWidth,
                    mapBounds.y + y * cellHeight,
                    cellWidth,
                    cellHeight
                );
                nodes.Add(new BSPNode(cell));
            }
        }

        return nodes;
    }
    
    public static void CreateRoomsInGrid(List<BSPNode> nodes, int padding = 2)
    {
        foreach (var node in nodes)
        {
            int roomWidth = node.Bounds.width - padding * 2;
            int roomHeight = node.Bounds.height - padding * 2;

            node.Room = new RectInt(
                node.Bounds.x + padding,
                node.Bounds.y + padding,
                roomWidth,
                roomHeight
            );
        }
    }

    public static List<RectInt> CreateGridCorridors(List<BSPNode> nodes, int rows, int cols, int corridorWidth = 3)
    {
        List<RectInt> corridors = new List<RectInt>();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = y * cols + x;
                BSPNode current = nodes[index];
                if (!current.Room.HasValue) continue; 
                Vector2Int center = GetRoomCenter(current);

                // Connect to right neighbor
                if (x < cols - 1)
                {
                    BSPNode right = nodes[index + 1];
                    if (right.Room.HasValue)
                    {
                        Vector2Int rightCenter = GetRoomCenter(right);
                        int half = corridorWidth / 2;

                        // Horizontal corridor
                        corridors.Add(new RectInt(
                            Mathf.Min(center.x, rightCenter.x),
                            center.y - half,
                            Mathf.Abs(center.x - rightCenter.x) + 1,
                            corridorWidth
                        ));
                    }
                }

                // Connect to bottom neighbor
                if (y < rows - 1)
                {
                    BSPNode bottom = nodes[index + cols];
                    if (bottom.Room.HasValue)
                    {
                        Vector2Int bottomCenter = GetRoomCenter(bottom);
                        int half = corridorWidth / 2;

                        // Vertical corridor
                        corridors.Add(new RectInt(
                            center.x - half,
                            Mathf.Min(center.y, bottomCenter.y),
                            corridorWidth,
                            Mathf.Abs(center.y - bottomCenter.y) + 1
                        ));
                    }
                }
            }
        }

        return corridors;
    }

    private static Vector2Int GetRoomCenter(BSPNode node)
    {
        RectInt room = node.Room.Value;
        return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
    }
    
    public static void RemoveRandomCorridors(List<RectInt> corridors, float keepChance = 0.5f)
    {
        for (int i = corridors.Count - 1; i >= 0; i--)
        {
            if (Random.value > keepChance)
            {
                corridors.RemoveAt(i);
            }
        }
    }

    public static void RemoveRandomRooms(List<BSPNode> nodes, float amount)
    {
        // Get all nodes that currently have a room
        List<BSPNode> nodesWithRooms = new List<BSPNode>();
        foreach (var node in nodes)
        {
            if (node.Room.HasValue)
                nodesWithRooms.Add(node);
        }

        int totalRooms = nodesWithRooms.Count;
        int roomsToRemove = (int)(totalRooms * amount); 

        // Shuffle the list randomly
        for (int i = 0; i < nodesWithRooms.Count; i++)
        {
            int randIndex = Random.Range(i, nodesWithRooms.Count);
            BSPNode temp = nodesWithRooms[i];
            nodesWithRooms[i] = nodesWithRooms[randIndex];
            nodesWithRooms[randIndex] = temp;
        }

        for (int i = 0; i < roomsToRemove; i++)
        {
            nodesWithRooms[i].Room = null;
        }
    }

    public static void KeepStartingRoom(BSPNode startRoom, int cols, int padding)
    {
        if (!startRoom.Room.HasValue)
        {
            RectInt bounds = startRoom.Bounds;
            startRoom.Room = new RectInt(
                bounds.x + padding,
                bounds.y + padding,
                bounds.width - padding * 2,
                bounds.height - padding * 2
            );
        }
    }

    public static List<RectInt> ConnectUnreachableRooms(BSPNode startRoom, List<BSPNode> nodes, int cols, int rows, int corridorWidth)
    {
        HashSet<BSPNode> reachable = new HashSet<BSPNode>();
        Queue<BSPNode> queue = new Queue<BSPNode>();

        queue.Enqueue(startRoom);
        reachable.Add(startRoom);

        while (queue.Count > 0)
        {
            BSPNode current = queue.Dequeue();

            int index = nodes.IndexOf(current);
            int x = index % cols;
            int y = index / cols;

            int[,] offsets = new int[,] { {0,1}, {0,-1}, {-1,0}, {1,0} };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + offsets[i,0];
                int ny = y + offsets[i,1];

                if (nx < 0 || nx >= cols || ny < 0 || ny >= rows) continue;

                int neighborIndex = ny * cols + nx;
                BSPNode neighbor = nodes[neighborIndex];

                if (neighbor.Room.HasValue && !reachable.Contains(neighbor))
                {
                    reachable.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        List<BSPNode> unreachableRooms = new List<BSPNode>();
        foreach (var node in nodes)
        {
            if (node.Room.HasValue && !reachable.Contains(node))
                unreachableRooms.Add(node);
        }
        
        List<RectInt> extraCorridors = new List<RectInt>();

        foreach (var unreachable in unreachableRooms)
        {
            BSPNode nearest = null;
            float minDist = float.MaxValue;
            Vector2Int urCenter = GetRoomCenter(unreachable);

            foreach (var r in reachable)
            {
                Vector2Int rCenter = GetRoomCenter(r);
                float dist = Vector2Int.Distance(urCenter, rCenter);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = r;
                }
            }

            if (nearest != null)
            {
                CreateCorridor(unreachable.Room.Value, nearest.Room.Value, extraCorridors, corridorWidth);

                reachable.Add(unreachable);
            }
        }

        return extraCorridors;
    }
}
