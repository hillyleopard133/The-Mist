using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TempleGeneration : MonoBehaviour
{
    [SerializeField] private Vector2Int roomSize;
    [SerializeField] private Vector2Int templeSize;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private int maxRoomNumber;
    [SerializeField] private int minRoomNumber;
    [SerializeField] private int maxBuildAttempts;

    private Room startingRoom;
    private List<Room> rooms;
    private GameObject[,] grid;
    private int buildAttempts;

    private void Start()
    {
        buildAttempts = 0;
        CreateGrid();
    }

    private void CreateGrid()
    {
        buildAttempts++;
        grid = new GameObject[templeSize.x, templeSize.y];
        int startingX = (templeSize.x - 1) / 2;
        int startingY = (templeSize.y - 1) / 2;
        
        Vector3 startPos = new Vector3(startingX * (roomSize.x + 1), startingY * (roomSize.y + 1), 0);
        
        rooms = new List<Room>();
        GameObject startingRoomObject = Instantiate(startingRoomPrefab,startPos, Quaternion.identity, transform);
        grid[startingX, startingY] = startingRoomObject;
        startingRoom = new Room(startingX, startingY, startingRoomObject);
        rooms.Add(startingRoom);
        GenerateNeighbours(startingRoom);

        if (rooms.Count < minRoomNumber)
        {
            if (buildAttempts >= maxBuildAttempts)
            {
                Debug.Log("Max builds reached");
                return;
            } 
            Debug.Log(rooms.Count + "Retrying");
            
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            CreateGrid();
        }
    }

    private void GenerateNeighbours(Room room)
    {
        if(rooms.Count >= maxRoomNumber) return;
        
        int gridX = room.gridX;
        int gridY = room.gridY;

        if (room.roomLeft == null)
        {
            if (gridX - 1 >= 0)
            {
                if (grid[gridX - 1, gridY] == null)
                {
                    int spawns = Random.Range(0, 2);
                    if (spawns == 0)
                    {
                        SpawnRoom(gridX - 1, gridY, room, Directions.Left);
                    }
                }
            }
        }

        if (room.roomRight == null)
        {
            if (gridX + 1 < templeSize.x)
            {
                if (grid[gridX + 1, gridY] == null)
                {
                    int spawns = Random.Range(0, 2);
                    if (spawns == 0)
                    {
                        SpawnRoom(gridX + 1, gridY, room, Directions.Right);
                    }
                }
            }
        }

        if (room.roomTop == null)
        {
            if (gridY + 1 < templeSize.y)
            {
                if (grid[gridX, gridY + 1] == null)
                {
                    int spawns = Random.Range(0, 2);
                    if (spawns == 0)
                    {
                        SpawnRoom(gridX, gridY + 1, room, Directions.Top);
                    }
                }
            }
        }

        if (room.roomBottom == null)
        {
            if (gridY - 1 >= 0)
            {
                if (grid[gridX, gridY - 1] == null)
                {
                    int spawns = Random.Range(0, 2);
                    if (spawns == 0)
                    {
                        SpawnRoom(gridX, gridY - 1, room, Directions.Bottom);
                    }
                }
            }
        }
        
        int roomIndex = rooms.IndexOf(room);
        if (rooms.Count > roomIndex + 1)
        {
            if (rooms.Count >= maxRoomNumber) return;
            GenerateNeighbours(rooms[roomIndex + 1]);
        }
    }

    private void SpawnRoom(int gridX, int gridY, Room room, Directions direction)
    {
        if(rooms.Count >= maxRoomNumber) return;
        
        Vector3 position = new Vector3(
            gridX* (roomSize.x + 1),
            gridY * (roomSize.y + 1),
            0
        );
                    
        GameObject newRoomObject = Instantiate(roomPrefab, position, Quaternion.identity, transform);
        grid[gridX, gridY] = newRoomObject;
        Room newRoom = new Room(gridX, gridY, newRoomObject);

        switch (direction)
        {
            case Directions.Left:
                room.roomLeft = newRoom;
                room.room.GetComponent<Doors>().OpenLeftDoor();
                newRoom.roomRight = room;
                newRoom.room.GetComponent<Doors>().OpenRightDoor();
                break;
            case Directions.Right:
                room.roomRight = newRoom;
                room.room.GetComponent<Doors>().OpenRightDoor();
                newRoom.roomLeft = room;
                newRoom.room.GetComponent<Doors>().OpenLeftDoor();
                break;
            case Directions.Top:
                room.roomTop = newRoom;
                room.room.GetComponent<Doors>().OpenTopDoor();
                newRoom.roomBottom = room;
                newRoom.room.GetComponent<Doors>().OpenBottomDoor();
                break;
            case Directions.Bottom:
                room.roomBottom = newRoom;
                room.room.GetComponent<Doors>().OpenBottomDoor();
                newRoom.roomTop = room;
                newRoom.room.GetComponent<Doors>().OpenTopDoor();
                break;
        }
        
        rooms.Add(newRoom);
    }
}
