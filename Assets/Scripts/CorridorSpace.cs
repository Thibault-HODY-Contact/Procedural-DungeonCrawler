using System.Collections.Generic;
using UnityEngine;

public class CorridorSpace
{
    const int SpacingAroundObstacles = 1;

    readonly List<BSPNode> rooms;
    readonly List<CorridorPiece> corridors;

    public CorridorSpace(List<BSPNode> rooms, List<CorridorPiece> corridors)
    {
        this.rooms = rooms;
        this.corridors = corridors;
    }

    public bool IsAreaFree(RectInt area, BSPNode roomA, BSPNode roomB)
    {
        if (area.width <= 0 || area.height <= 0) return true;

        foreach (BSPNode room in rooms)
        {
            bool isConnectedRoom = room == roomA || room == roomB;
            RectInt blockedArea = isConnectedRoom ? room.roomBounds : Expand(room.roomBounds, SpacingAroundObstacles);
            if (area.Overlaps(blockedArea)) return false;
        }

        foreach (CorridorPiece existing in corridors)
            if (area.Overlaps(Expand(existing.area, SpacingAroundObstacles))) return false;

        return true;
    }

    static RectInt Expand(RectInt bounds, int amount)
    {
        return new RectInt(bounds.x - amount, bounds.y - amount, bounds.width + amount * 2, bounds.height + amount * 2);
    }
}