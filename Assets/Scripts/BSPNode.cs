using UnityEngine;

public class BSPNode 
{
    public RectInt bounds;
    public BSPNode left, right;
    public RectInt? roomBound;
    public float roomHeight;
    public RoomStyleSO style;
    public bool IsLeaf => left == null && right == null;

}
