using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class TempleGeneration : MonoBehaviour
{
    [SerializeField] private Vector2Int roomSize;
    [SerializeField] private Vector2Int templeSize;
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private GameObject bigChestRoom;
    [SerializeField] private GameObject smallChestRoom;
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private GameObject relicRoomPrefab;
    [SerializeField] private GameObject treasureRoomPrefab;
    [SerializeField] private int maxRoomNumber;
    [SerializeField] private int minRoomNumber;
    [SerializeField] private int maxBuildAttempts;
    [SerializeField] private int numberOfBigTreasureRooms;
    [SerializeField] private int numberOfSmallTreasureRooms;

    private Room startingRoom;
    private Room bossRoom;
    private List<Room> rooms;
    private GameObject[,] grid;
    private int buildAttempts;
    private int requiredEndRooms = 4;
    
    public static TempleGeneration Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

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
        
        List<Room> endRooms = GetEndRooms();

        if (rooms.Count < minRoomNumber || endRooms.Count < requiredEndRooms)
        {
            if (buildAttempts >= maxBuildAttempts) return;
            
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            CreateGrid();
            return;
        }
        
        ReplaceEndRooms(endRooms);
        AddTreasureRooms(endRooms);
        
        SceneEnemies.Instance.InitiateEnemyAreas();
        Player.Instance.gameObject.transform.position = startingRoomObject.transform.position + new Vector3(roomSize.x / 2f, roomSize.y / 2f, 0f);;
    }
    
    public void UnlockBossRoom()
    {
        bossRoom.roomObject.GetComponent<BossDoors>().OpenBossDoors();
    }

    private void AddTreasureRooms(List<Room> endRooms)
    {
        List<Room> availableRooms = rooms.Where(r => !endRooms.Contains(r)).ToList();
        
        List<Room> bigTreasureRooms = new List<Room>();
        List<Room> smallTreasureRooms = new List<Room>();
        for (int i = 0; i < numberOfBigTreasureRooms; i++)
        {
            Room room = availableRooms[Random.Range(1, availableRooms.Count)];
            bigTreasureRooms.Add(room);
            availableRooms.Remove(room);
        }
        for (int i = 0; i < numberOfSmallTreasureRooms; i++)
        {
            Room room = availableRooms[Random.Range(1, availableRooms.Count)];
            smallTreasureRooms.Add(room);
            availableRooms.Remove(room);
        }
        
        ReplaceTreasureRooms(bigTreasureRooms, smallTreasureRooms);
    }

    private void ReplaceTreasureRooms(List<Room> bigTreasureRooms, List<Room> smallTreasureRooms)
    {
        foreach (Room room in bigTreasureRooms)
        {
            ReplaceRoom(room, bigChestRoom);
        }

        foreach (Room room in smallTreasureRooms)
        {
            ReplaceRoom(room, smallChestRoom);
        }
    }

    private void ReplaceRoom(Room room, GameObject roomPrefab)
    {
        List<Directions> openDoors = room.AllOpenDoors();
        Destroy(room.roomObject);
            
        Vector3 position = new Vector3(
            room.gridX * (roomSize.x + 1),
            room.gridY * (roomSize.y + 1),
            0
        );
            
        GameObject newRoomObject = Instantiate(roomPrefab, position, Quaternion.identity, transform);
        grid[room.gridX, room.gridY] = newRoomObject;
        room.roomObject = newRoomObject;

        foreach (Directions direction in openDoors)
        {
            switch (direction)
            {
                case Directions.Left:
                    room.roomObject.GetComponent<Doors>().OpenLeftDoor();
                    break;
                case Directions.Right:
                    room.roomObject.GetComponent<Doors>().OpenRightDoor();
                    break;
                case Directions.Top:
                    room.roomObject.GetComponent<Doors>().OpenTopDoor();
                    break;
                case Directions.Bottom:
                    room.roomObject.GetComponent<Doors>().OpenBottomDoor();
                    break;
            }
        }
    }

    private void ReplaceEndRooms(List<Room> endRooms)
    {
        if (endRooms.Count > requiredEndRooms)
        {
            while (endRooms.Count > requiredEndRooms)
            {
                endRooms.RemoveAt(0);
            }
        }

        for (int i = 0; i < endRooms.Count - 1; i++)
        {
            Room room = endRooms[i];
            //Directions direction = room.NeighbourDirection();
            //SpawnEndRoom(room, direction, relicRoomPrefab);
            ReplaceRoom(room, relicRoomPrefab);
        }
        
        bossRoom = endRooms[^1];
        //Directions bossDirection = bossRoom.NeighbourDirection();
        //SpawnEndRoom(bossRoom, bossDirection, bossRoomPrefab);
        ReplaceRoom(bossRoom, bossRoomPrefab);
    }

    private void SpawnEndRoom(Room room, Directions direction, GameObject prefab)
    {
        Destroy(room.roomObject);
        
        Vector3 position = new Vector3(
            room.gridX * (roomSize.x + 1),
            room.gridY * (roomSize.y + 1),
            0
        );
                    
        GameObject newRoomObject = Instantiate(prefab, position, Quaternion.identity, transform);
        grid[room.gridX, room.gridY] = newRoomObject;
        room.roomObject = newRoomObject;
        
        switch (direction)
        {
            case Directions.Left:
                room.roomObject.GetComponent<Doors>().OpenLeftDoor();
                break;
            case Directions.Right:
                room.roomObject.GetComponent<Doors>().OpenRightDoor();
                break;
            case Directions.Top:
                room.roomObject.GetComponent<Doors>().OpenTopDoor();
                break;
            case Directions.Bottom:
                room.roomObject.GetComponent<Doors>().OpenBottomDoor();
                break;
        }
    }

    private List<Room> GetEndRooms()
    {
        List<Room> endRooms = new List<Room>();
        foreach (Room room in rooms)
        {
            if(room.isEndRoom) endRooms.Add(room);
        }
        return endRooms;
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
                        int randomIndex = Random.Range(0, roomPrefabs.Length);
                        SpawnRoom(gridX - 1, gridY, room, Directions.Left, roomPrefabs[randomIndex]);
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
                        int randomIndex = Random.Range(0, roomPrefabs.Length);
                        SpawnRoom(gridX + 1, gridY, room, Directions.Right, roomPrefabs[randomIndex]);
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
                        int randomIndex = Random.Range(0, roomPrefabs.Length);
                        SpawnRoom(gridX, gridY + 1, room, Directions.Top, roomPrefabs[randomIndex]);
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
                        int randomIndex = Random.Range(0, roomPrefabs.Length);
                        SpawnRoom(gridX, gridY - 1, room, Directions.Bottom, roomPrefabs[randomIndex]);
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

    private void SpawnRoom(int gridX, int gridY, Room room, Directions direction, GameObject prefab)
    {
        if(rooms.Count >= maxRoomNumber) return;
        
        Vector3 position = new Vector3(
            gridX* (roomSize.x + 1),
            gridY * (roomSize.y + 1),
            0
        );
                    
        GameObject newRoomObject = Instantiate(prefab, position, Quaternion.identity, transform);
        grid[gridX, gridY] = newRoomObject;
        Room newRoom = new Room(gridX, gridY, newRoomObject);

        switch (direction)
        {
            case Directions.Left:
                room.roomLeft = newRoom;
                room.roomObject.GetComponent<Doors>().OpenLeftDoor();
                room.isEndRoom = false;
                newRoom.roomRight = room;
                newRoom.roomObject.GetComponent<Doors>().OpenRightDoor();
                break;
            case Directions.Right:
                room.roomRight = newRoom;
                room.roomObject.GetComponent<Doors>().OpenRightDoor();
                room.isEndRoom = false;
                newRoom.roomLeft = room;
                newRoom.roomObject.GetComponent<Doors>().OpenLeftDoor();
                break;
            case Directions.Top:
                room.roomTop = newRoom;
                room.roomObject.GetComponent<Doors>().OpenTopDoor();
                room.isEndRoom = false;
                newRoom.roomBottom = room;
                newRoom.roomObject.GetComponent<Doors>().OpenBottomDoor();
                break;
            case Directions.Bottom:
                room.roomBottom = newRoom;
                room.roomObject.GetComponent<Doors>().OpenBottomDoor();
                room.isEndRoom = false;
                newRoom.roomTop = room;
                newRoom.roomObject.GetComponent<Doors>().OpenTopDoor();
                break;
        }
        
        rooms.Add(newRoom);
    }
}
