using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPNode
{
    public RectInt Bounds;
    public BSPNode Left;
    public BSPNode Right;
    public RectInt? Room;

    public bool IsLeaf => Left == null && Right == null;

    public BSPNode(RectInt bounds)
    {
        Bounds = bounds;
    }
}
