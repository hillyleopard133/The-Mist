using UnityEngine;

public enum Directions
{
    Right,
    Left,
    Top,
    Bottom
}

public class Room
{
    public Room roomTop;
    public Room roomBottom;
    public Room roomLeft;
    public Room roomRight;

    public int gridX;
    public int gridY;
    public GameObject room;

    public Room(int gridX, int gridY, GameObject room)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.room = room;
    }
    
    public bool IsEndRoom()
    {
        return NumberOfNeighbours() < 2;
    }

    private int NumberOfNeighbours()
    {
        int neighbours = 0;
        
        if(roomTop != null) neighbours += 1;
        if(roomBottom != null) neighbours += 1;
        if(roomLeft != null) neighbours += 1;
        if(roomRight != null) neighbours += 1;
        
        return neighbours;
    }
}