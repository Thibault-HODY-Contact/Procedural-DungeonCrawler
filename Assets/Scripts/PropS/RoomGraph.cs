using System.Collections.Generic;
using UnityEngine;

public static class RoomGraph
{
    struct Link
    {
        public BSPNode room;
        public Vector2 door;
    }

    public static void ComputeDistanceToExit(List<BSPNode> rooms, List<CorridorPiece> corridors, BSPNode exitRoom)
    {
        if (exitRoom == null) return;

        Dictionary<BSPNode, List<Link>> links = BuildLinks(rooms, corridors);
        Dictionary<BSPNode, int> steps = new Dictionary<BSPNode, int> { [exitRoom] = 0 };
        exitRoom.doorTowardExit = exitRoom.roomBounds.center;

        Queue<BSPNode> toVisit = new Queue<BSPNode>();
        toVisit.Enqueue(exitRoom);
        while (toVisit.Count > 0)
        {
            BSPNode room = toVisit.Dequeue();
            foreach (Link link in links[room])
            {
                if (steps.ContainsKey(link.room)) continue;
                steps[link.room] = steps[room] + 1;
                link.room.doorTowardExit = links[link.room].Find(back => back.room == room).door;
                toVisit.Enqueue(link.room);
            }
        }

        int farthest = 1;
        foreach (int value in steps.Values)
            farthest = Mathf.Max(farthest, value);

        foreach (BSPNode room in rooms)
        {
            if (steps.TryGetValue(room, out int value))
            {
                room.distanceToExit = (float)value / farthest;
                continue;
            }
            room.distanceToExit = 1f;
            room.doorTowardExit = room.roomBounds.center;
        }
    }

    static Dictionary<BSPNode, List<Link>> BuildLinks(List<BSPNode> rooms, List<CorridorPiece> corridors)
    {
        Dictionary<BSPNode, List<Link>> links = new Dictionary<BSPNode, List<Link>>();
        foreach (BSPNode room in rooms)
            links[room] = new List<Link>();

        foreach (List<CorridorPiece> corridor in GroupConnectedPieces(corridors))
        {
            List<BSPNode> linkedRooms = rooms.FindAll(room => corridor.Exists(piece => Touches(piece.area, room.roomBounds)));
            foreach (BSPNode roomA in linkedRooms)
                foreach (BSPNode roomB in linkedRooms)
                    if (roomA != roomB) links[roomA].Add(new Link { room = roomB, door = FindDoor(corridor, roomA) });
        }
        return links;
    }

    static Vector2 FindDoor(List<CorridorPiece> corridor, BSPNode room)
    {
        RectInt piece = corridor.Find(p => Touches(p.area, room.roomBounds)).area;
        RectInt bounds = room.roomBounds;
        float x = (Mathf.Max(piece.xMin, bounds.xMin) + Mathf.Min(piece.xMax, bounds.xMax)) / 2f;
        float y = (Mathf.Max(piece.yMin, bounds.yMin) + Mathf.Min(piece.yMax, bounds.yMax)) / 2f;
        return new Vector2(x, y);
    }

    static List<List<CorridorPiece>> GroupConnectedPieces(List<CorridorPiece> corridors)
    {
        List<List<CorridorPiece>> groups = new List<List<CorridorPiece>>();
        List<CorridorPiece> remaining = new List<CorridorPiece>(corridors);

        while (remaining.Count > 0)
        {
            List<CorridorPiece> group = new List<CorridorPiece> { remaining[0] };
            remaining.RemoveAt(0);
            for (int i = 0; i < group.Count; i++)
            {
                CorridorPiece current = group[i];
                List<CorridorPiece> touching = remaining.FindAll(other => Touches(current.area, other.area));
                group.AddRange(touching);
                remaining.RemoveAll(touching.Contains);
            }
            groups.Add(group);
        }
        return groups;
    }

    static bool Touches(RectInt a, RectInt b)
    {
        int overlapX = Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin);
        int overlapY = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
        return overlapX >= 0 && overlapY >= 0 && (overlapX > 0 || overlapY > 0);
    }
}