using System.Collections.Generic;
using UnityEngine;

public class RoomOccupancy
{
    struct TakenSpot
    {
        public Vector2 position;
        public float radius;
    }

    readonly List<CorridorPiece> corridors;
    readonly List<Vector2> reservedSpots;
    readonly List<TakenSpot> takenSpots = new List<TakenSpot>();

    public RoomOccupancy(List<CorridorPiece> corridors, List<Vector2> reservedSpots)
    {
        this.corridors = corridors;
        this.reservedSpots = reservedSpots;
    }

    public bool IsFree(Vector2 spot, float radius, float passageClearance, float spacing)
    {
        foreach (CorridorPiece corridor in corridors)
            if (Grow(corridor.area, passageClearance + radius).Contains(spot)) return false;

        foreach (Vector2 reserved in reservedSpots)
            if (Vector2.Distance(reserved, spot) < passageClearance + radius) return false;

        foreach (TakenSpot taken in takenSpots)
            if (Vector2.Distance(taken.position, spot) < taken.radius + radius + spacing) return false;

        return true;
    }

    public void Take(Vector2 spot, float radius)
    {
        takenSpots.Add(new TakenSpot { position = spot, radius = radius });
    }

    static Rect Grow(RectInt area, float amount)
    {
        return new Rect(area.x - amount, area.y - amount, area.width + amount * 2f, area.height + amount * 2f);
    }
}