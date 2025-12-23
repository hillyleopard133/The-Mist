using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPGen: MonoBehaviour
{
    private List<RectInt> corridors;
    private BSPNode root;
    
    private PlayerActions actions;
    
    private int mapWidth = 100;
    private int mapHeight = 100;
    private int rows = 5;      
    private int cols = 5;     
    private int roomPadding = 4;
    private int corridorWidth = 3;


    private void Awake()
    {
        actions = new PlayerActions();
    }
    
    private void Start()
    {
        actions.BSP.Generate.performed += ctx => GenerateGrid(); 
    }
    
    public BSPNode GenerateGrid()
    {
        RoomPainter.Instance.Clear();

        root = new BSPNode(new RectInt(0, 0, mapWidth, mapHeight));

        List<BSPNode> nodes = BSPAlgorithm.CreateGridNodes(root.Bounds, rows, cols);

        BSPAlgorithm.CreateRoomsInGrid(nodes, roomPadding);
        BSPAlgorithm.RemoveRandomRooms(nodes, 0.5f);
        
        int bottomRow = 0; 
        int middleCol = cols / 2;
        int startIndex = bottomRow * cols + middleCol;
        BSPNode startRoom = nodes[startIndex];
        
        BSPAlgorithm.KeepStartingRoom(startRoom, cols, roomPadding);

        corridors = BSPAlgorithm.CreateGridCorridors(nodes, rows, cols, corridorWidth);
        corridors.AddRange(BSPAlgorithm.ConnectUnreachableRooms(startRoom, nodes, cols, rows, corridorWidth));
        
        RoomPainter.Instance.Paint(nodes, corridors);

        return root;
    }

    
    public BSPNode Generate(int width, int height)
    {
        RoomPainter.Instance.Clear();

        root = new BSPNode(new RectInt(0, 0, width, height));
        BSPAlgorithm.Split(root, 17, 7);
        BSPAlgorithm.CreateRooms(root,minRoomWidth: 12, maxRoomWidth: 15, minRoomHeight: 12, maxRoomHeight: 15);
        
        corridors = new List<RectInt>();
        BSPAlgorithm.CreateCorridors(root, corridors);
        
        RoomPainter.Instance.Paint(root, corridors);

        return root;
    }
    
    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }
    
}
