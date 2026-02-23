using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSPGen: MonoBehaviour
{
    private List<RectInt> corridors;
    private BSPNode root;
    private BSPNode startingRoom;
    
    private PlayerActions actions;
    
    private const int mapWidth = 150;
    private const int mapHeight = 150;
    private const int rows = 5;
    private const int cols = 5;
    private const int roomPadding = 4;
    private const int corridorWidth = 3;

    [SerializeField] private GameObject[] enemies;

    [SerializeField] private AStarArea aStarArea;

    private void Awake()
    {
        actions = new PlayerActions();
    }
    
    private void Start()
    {
        actions.BSP.Generate.performed += ctx => Generate(); 
    }
    
    public BSPNode GenerateGrid()
    {
        RoomPainter.Instance.Clear();

        root = new BSPNode(new RectInt(0, 0, mapWidth, mapHeight));

        List<BSPNode> nodes = BSPAlgorithm.CreateGridNodes(root.Bounds, rows, cols);

        BSPAlgorithm.CreateRoomsInGrid(nodes, roomPadding);
        BSPAlgorithm.RemoveRandomRooms(nodes, 0.5f);
        
        BSPNode startRoom = nodes[cols/2];
        
        BSPAlgorithm.KeepStartingRoom(startRoom, cols, roomPadding);

        corridors = BSPAlgorithm.CreateGridCorridors(nodes, rows, cols, corridorWidth);
        corridors.AddRange(BSPAlgorithm.ConnectUnreachableRooms(startRoom, nodes, cols, rows, corridorWidth));
        
        RoomPainter.Instance.Paint(nodes, corridors);

        return root;
    }

    public BSPNode GenerateTemple()
    {
        RoomPainter.Instance.Clear();
        root = new BSPNode(new RectInt(0, 0, mapWidth, mapHeight));
        
        BSPAlgorithm.Split(root, 17, 7);
        BSPAlgorithm.CreateRooms(root,minRoomWidth: 12, maxRoomWidth: 15, minRoomHeight: 12, maxRoomHeight: 15);
        
        List<BSPNode> leaves = BSPAlgorithm.GetLeafNodes(root);
        
        int cols = Mathf.CeilToInt(Mathf.Sqrt(leaves.Count)); 
        int rows = Mathf.CeilToInt(leaves.Count / (float)cols);
        int bottomRow = rows - 1;
        int startIndex = bottomRow * cols + cols / 2;
        BSPNode startRoom = leaves[Mathf.Min(startIndex, leaves.Count - 1)];
        
        BSPAlgorithm.KeepStartingRoom(startRoom, cols, roomPadding);
        
        List<RectInt> corridors = BSPAlgorithm.CreateGridCorridors(leaves, rows, cols);
        corridors.AddRange(BSPAlgorithm.ConnectUnreachableRooms(startRoom, leaves, cols, rows, corridorWidth));
        
        RoomPainter.Instance.Paint(root, corridors);

        return root;
    }
    
    public BSPNode Generate()
    {
        RoomPainter.Instance.Clear();

        root = new BSPNode(new RectInt(0, 0, mapWidth, mapHeight));
        BSPAlgorithm.Split(root, 17, 7);
        BSPAlgorithm.CreateRooms(root,minRoomWidth: 12, maxRoomWidth: 15, minRoomHeight: 12, maxRoomHeight: 15);
        
        corridors = new List<RectInt>();
        BSPAlgorithm.CreateCorridorsBetter(root, corridors, corridorWidth);
        
        RoomPainter.Instance.Paint(root, corridors);
        
        List<RectInt> rooms = BSPAlgorithm.GetLeafNodes(root)
            .Where(n => n.Room.HasValue)
            .Select(n => n.Room.Value)
            .ToList();
        
        RectInt mapBounds = new RectInt(0, 0, mapWidth, mapHeight);
        RoomPainter.Instance.FillCollision(rooms, corridors, mapBounds);
        
        startingRoom = BSPAlgorithm.GetStartingRoom(root, mapWidth, mapHeight);

        aStarArea.AddObstaclesAndPreferredPaths();

        SpawnPlayer();
        SpawnEnemies();
        
        return root;
    }

    private void SpawnPlayer()
    {
        RectInt room = startingRoom.Room.Value;
        
        Vector2Int spawnCell = new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);

        Vector3 worldPos = new Vector3(spawnCell.x, spawnCell.y, 0);
        
        Player.Instance.gameObject.transform.position = worldPos;
    }

    private void SpawnEnemies()
    {
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in existingEnemies)
        {
            Destroy(enemy);
        }
        
        List<BSPNode> leaves = BSPAlgorithm.GetLeafNodes(root);

        foreach (var room in leaves)
        {
            if (room == startingRoom) continue; // skip starting room

            EnemySpawner.SpawnEnemiesInRoom(room, enemies, minEnemies: 1, maxEnemies: 1, padding: 1);
        }
    }
    
    private void OnEnable()
    {
        actions.BSP.Enable();
    }

    private void OnDisable()
    {
        actions.BSP.Disable();
    }
    
}
