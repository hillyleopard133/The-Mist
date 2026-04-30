using System.Collections.Generic;
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

    public List<Directions> AllOpenDoors()
    {
        List<Directions> openDoors = new List<Directions>();
        if(roomTop != null) openDoors.Add(Directions.Top);
        if(roomBottom != null) openDoors.Add(Directions.Bottom);
        if(roomLeft != null) openDoors.Add(Directions.Left);
        if(roomRight != null) openDoors.Add(Directions.Right);
        return openDoors;
    }
}