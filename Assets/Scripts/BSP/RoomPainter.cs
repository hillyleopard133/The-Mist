using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPainter : Singleton<RoomPainter>
{
    [SerializeField] Tilemap floorMap;
    [SerializeField] Tilemap wallMap;
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase wallTile;

    public void Paint(BSPNode root, List<RectInt> corridors)
    {
        PaintRooms(root);
        PaintCorridors(corridors);
        PaintWalls();
    }
    
    public void Clear()
    {
        floorMap.ClearAllTiles();
        wallMap.ClearAllTiles();
    }

    void PaintRooms(BSPNode node)
    {
        if (node == null) return;

        if (node.Room.HasValue)
        {
            RectInt r = node.Room.Value;
            for (int x = r.x; x < r.xMax; x++)
            for (int y = r.y; y < r.yMax; y++)
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
        }

        PaintRooms(node.Left);
        PaintRooms(node.Right);
    }

    void PaintCorridors(List<RectInt> corridors)
    {
        foreach (RectInt c in corridors)
            for (int x = c.x; x < c.xMax; x++)
            for (int y = c.y; y < c.yMax; y++)
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
    }

    void PaintWalls()
    {
        foreach (Vector3Int pos in floorMap.cellBounds.allPositionsWithin)
        {
            if (!floorMap.HasTile(pos)) continue;

            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector3Int n = pos + new Vector3Int(dx, dy, 0);
                if (!floorMap.HasTile(n))
                    wallMap.SetTile(n, wallTile);
            }
        }
    }
}
