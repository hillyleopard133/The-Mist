using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPainter : Singleton<RoomPainter>
{
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private Tilemap collisionMap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase collisionTile;
    [SerializeField] private TileBase pathTile;

    public void Paint(BSPNode root, List<RectInt> corridors)
    {
        PaintRooms(root);
        PaintCorridors(corridors);
        PaintWalls();
    }
    
    public void Paint(List<BSPNode> nodes, List<RectInt> corridors)
    {
        PaintRooms(nodes);
        PaintCorridors(corridors);
        PaintWalls();
    }

    void PaintRooms(List<BSPNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (!node.Room.HasValue) continue;

            RectInt r = node.Room.Value;
            for (int x = r.x; x < r.xMax; x++)
            for (int y = r.y; y < r.yMax; y++)
            {
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
                collisionMap.SetTile(new Vector3Int(x, y, 0), pathTile);
            }
        }
    }
    
    public void Clear()
    {
        floorMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        collisionMap.ClearAllTiles();
    }

    public void PaintRooms(BSPNode node)
    {
        if (node == null) return;

        if (node.Room.HasValue)
        {
            RectInt r = node.Room.Value;
            for (int x = r.x; x < r.xMax; x++)
            for (int y = r.y; y < r.yMax; y++)
            {
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
                collisionMap.SetTile(new Vector3Int(x, y, 0), pathTile);
            }
        }

        PaintRooms(node.Left);
        PaintRooms(node.Right);
    }

    private void PaintCorridors(List<RectInt> corridors)
    {
        foreach (RectInt c in corridors)
            for (int x = c.x; x < c.xMax; x++)
            for (int y = c.y; y < c.yMax; y++)
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
    }

    private void PaintWalls()
    {
        foreach (Vector3Int pos in floorMap.cellBounds.allPositionsWithin)
        {
            if (!floorMap.HasTile(pos)) continue;

            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector3Int n = pos + new Vector3Int(dx, dy, 0);
                if (!floorMap.HasTile(n))
                {
                    wallMap.SetTile(n, wallTile);
                    collisionMap.SetTile(n, collisionTile);
                }
            }
        }
    }
    
    public void FillCollision(List<RectInt> rooms, List<RectInt> corridors, RectInt mapBounds)
    {
        for (int x = mapBounds.x; x < mapBounds.x + mapBounds.width; x++)
        {
            for (int y = mapBounds.y; y < mapBounds.y + mapBounds.height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // Skip if inside a room
                bool insideRoom = false;
                foreach (var room in rooms)
                {
                    if (x >= room.x && x < room.x + room.width && y >= room.y && y < room.y + room.height)
                    {
                        insideRoom = true;
                        break;
                    }
                }
                if (insideRoom) continue;

                // Skip if inside a corridor
                bool insideCorridor = false;
                foreach (var corridor in corridors)
                {
                    if (x >= corridor.x && x < corridor.x + corridor.width && y >= corridor.y && y < corridor.y + corridor.height)
                    {
                        insideCorridor = true;
                        break;
                    }
                }
                if (insideCorridor) continue;

                // Place collision tile
                collisionMap.SetTile(pos, collisionTile);
            }
        }
    }
}
