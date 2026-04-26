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
    public GameObject roomObject;

    public bool isEndRoom = true;

    public Room(int gridX, int gridY, GameObject roomObject)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.roomObject = roomObject;
    }
    
    public Directions NeighbourDirection()
    {
        if(roomTop != null) return Directions.Top;
        if(roomBottom != null) return Directions.Bottom;
        if(roomLeft != null) return Directions.Left;
        
        return Directions.Right;
    }
}