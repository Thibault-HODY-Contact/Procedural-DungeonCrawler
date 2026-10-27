using UnityEngine;

public class BSPNode
{
    public RectInt area;
    public BSPNode left;
    public BSPNode right;
    public RectInt roomBounds;
    public float floorHeight;
    public float distanceToExit;
    public Vector2 doorTowardExit;
    public RoomStyleSO style;

    public bool IsLeaf => left == null && right == null;
}