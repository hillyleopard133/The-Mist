using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPGen: MonoBehaviour
{
    private List<RectInt> corridors;
    private BSPNode root;
    
    private PlayerActions actions;

    private void Awake()
    {
        actions = new PlayerActions();
    }
    
    private void Start()
    {
        actions.BSP.Generate.performed += ctx => Generate(150, 150); 
    }
    
    public BSPNode Generate(int width, int height)
    {
        RoomPainter.Instance.Clear();

        root = new BSPNode(new RectInt(0, 0, width, height));
        BSPAlgorithm.Split(root, 17, 5);
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
