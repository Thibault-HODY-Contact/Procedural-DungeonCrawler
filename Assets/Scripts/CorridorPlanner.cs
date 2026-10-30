using System.Collections.Generic;
using UnityEngine;

public class CorridorPlanner
{
    const int MaxAttempts = 10;

    readonly BSPMAPSO settings;
    readonly List<CorridorPiece> corridors = new List<CorridorPiece>();
    readonly CorridorSpace space;

    public int FailedConnections { get; private set; }

    public CorridorPlanner(BSPMAPSO settings, List<BSPNode> rooms)
    {
        this.settings = settings;
        space = new CorridorSpace(rooms, corridors);
    }

    public List<CorridorPiece> ConnectAllRooms(BSPNode root, System.Random rng)
    {
        ConnectChildren(root, rng);
        return corridors;
    }

    void ConnectChildren(BSPNode node, System.Random rng)
    {
        if (node.IsLeaf) return;

        ConnectChildren(node.left, rng);
        ConnectChildren(node.right, rng);

        List<(BSPNode left, BSPNode right)> candidatePairs = GetPairsByDistance(node);
        bool preferElbow = settings.elbowPriority > 0 && rng.Next(100) < settings.elbowPriority;

        List<CorridorPiece> corridor = preferElbow ? FindElbowCorridor(candidatePairs, rng) : null;
        if (corridor == null) corridor = FindAnyCorridor(candidatePairs, rng);

        if (corridor == null) FailedConnections++;
        else corridors.AddRange(corridor.FindAll(piece => piece.area.width > 0 && piece.area.height > 0));
    }

    List<(BSPNode left, BSPNode right)> GetPairsByDistance(BSPNode node)
    {
        List<BSPNode> leftRooms = new List<BSPNode>();
        List<BSPNode> rightRooms = new List<BSPNode>();
        CollectLeafRooms(node.left, leftRooms);
        CollectLeafRooms(node.right, rightRooms);

        List<(BSPNode left, BSPNode right)> pairs = new List<(BSPNode left, BSPNode right)>();
        foreach (BSPNode leftRoom in leftRooms)
            foreach (BSPNode rightRoom in rightRooms)
                pairs.Add((leftRoom, rightRoom));
        pairs.Sort((first, second) =>
            DistanceBetween(first.left, first.right).CompareTo(DistanceBetween(second.left, second.right)));
        return pairs;
    }

    void CollectLeafRooms(BSPNode node, List<BSPNode> result)
    {
        if (node.IsLeaf)
        {
            result.Add(node);
            return;
        }
        CollectLeafRooms(node.left, result);
        CollectLeafRooms(node.right, result);
    }

    List<CorridorPiece> FindElbowCorridor(List<(BSPNode left, BSPNode right)> pairs, System.Random rng)
    {
        foreach (var pair in pairs)
        {
            List<CorridorPiece> elbowCorridor = TryElbowWithRetries(pair.left, pair.right, rng);
            if (elbowCorridor != null) return elbowCorridor;
        }
        return null;
    }

    List<CorridorPiece> FindAnyCorridor(List<(BSPNode left, BSPNode right)> pairs, System.Random rng)
    {
        foreach (var pair in pairs)
        {
            List<CorridorPiece> corridor = TryVerticalCorridor(pair.left, pair.right, rng)
                ?? TryHorizontalCorridor(pair.left, pair.right, rng)
                ?? TryElbowWithRetries(pair.left, pair.right, rng);
            if (corridor != null) return corridor;
        }
        return null;
    }

    List<CorridorPiece> TryElbowWithRetries(BSPNode roomA, BSPNode roomB, System.Random rng)
    {
        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            bool startFromA = rng.Next(2) == 0;
            List<CorridorPiece> elbowCorridor = startFromA ? TryElbowCorridor(roomA, roomB, rng) : TryElbowCorridor(roomB, roomA, rng);
            if (elbowCorridor != null) return elbowCorridor;
        }
        return null;
    }

    List<CorridorPiece> TryVerticalCorridor(BSPNode roomA, BSPNode roomB, System.Random rng)
    {
        RectInt boundsA = roomA.roomBounds;
        RectInt boundsB = roomB.roomBounds;
        int width = settings.corridorWidth;

        int overlapMin = Mathf.Max(boundsA.xMin, boundsB.xMin) + settings.corridorMargin;
        int overlapMax = Mathf.Min(boundsA.xMax, boundsB.xMax) - settings.corridorMargin;
        if (overlapMax - overlapMin < width) return null;

        bool aIsBelow = boundsA.yMax <= boundsB.yMin;
        RectInt lowerRoom = aIsBelow ? boundsA : boundsB;
        RectInt upperRoom = aIsBelow ? boundsB : boundsA;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            int x = rng.Next(overlapMin, overlapMax - width + 1);
            RectInt area = new RectInt(x, lowerRoom.yMax, width, upperRoom.yMin - lowerRoom.yMax);
            if (space.IsAreaFree(area, roomA, roomB))
                return new List<CorridorPiece> { new CorridorPiece { area = area, runsAlongX = false } };
        }
        return null;
    }

    List<CorridorPiece> TryHorizontalCorridor(BSPNode roomA, BSPNode roomB, System.Random rng)
    {
        RectInt boundsA = roomA.roomBounds;
        RectInt boundsB = roomB.roomBounds;
        int width = settings.corridorWidth;

        int overlapMin = Mathf.Max(boundsA.yMin, boundsB.yMin) + settings.corridorMargin;
        int overlapMax = Mathf.Min(boundsA.yMax, boundsB.yMax) - settings.corridorMargin;
        if (overlapMax - overlapMin < width) return null;

        bool aIsLeft = boundsA.xMax <= boundsB.xMin;
        RectInt leftRoom = aIsLeft ? boundsA : boundsB;
        RectInt rightRoom = aIsLeft ? boundsB : boundsA;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            int y = rng.Next(overlapMin, overlapMax - width + 1);
            RectInt area = new RectInt(leftRoom.xMax, y, rightRoom.xMin - leftRoom.xMax, width);
            if (space.IsAreaFree(area, roomA, roomB))
                return new List<CorridorPiece> { new CorridorPiece { area = area, runsAlongX = true } };
        }
        return null;
    }

    List<CorridorPiece> TryElbowCorridor(BSPNode fromRoom, BSPNode toRoom, System.Random rng)
    {
        RectInt fromBounds = fromRoom.roomBounds;
        RectInt toBounds = toRoom.roomBounds;
        int width = settings.corridorWidth;
        int margin = settings.corridorMargin;

        int elbowY = rng.Next(fromBounds.yMin + margin, fromBounds.yMax - width - margin + 1);
        int elbowX = rng.Next(toBounds.xMin + margin, toBounds.xMax - width - margin + 1);
        RectInt elbowCell = new RectInt(elbowX, elbowY, width, width);

        RectInt horizontalLeg;
        bool opensEast;
        if (elbowCell.xMin >= fromBounds.xMax) { horizontalLeg = new RectInt(fromBounds.xMax, elbowY, elbowCell.xMin - fromBounds.xMax, width); opensEast = false; }
        else if (elbowCell.xMax <= fromBounds.xMin) { horizontalLeg = new RectInt(elbowCell.xMax, elbowY, fromBounds.xMin - elbowCell.xMax, width); opensEast = true; }
        else return null;

        RectInt verticalLeg;
        bool opensNorth;
        if (elbowCell.yMax <= toBounds.yMin) { verticalLeg = new RectInt(elbowX, elbowCell.yMax, width, toBounds.yMin - elbowCell.yMax); opensNorth = true; }
        else if (elbowCell.yMin >= toBounds.yMax) { verticalLeg = new RectInt(elbowX, toBounds.yMax, width, elbowCell.yMin - toBounds.yMax); opensNorth = false; }
        else return null;

        bool hasBothLegs = horizontalLeg.width > 0 && verticalLeg.height > 0;
        bool pathIsFree = hasBothLegs
            && space.IsAreaFree(horizontalLeg, fromRoom, toRoom)
            && space.IsAreaFree(verticalLeg, fromRoom, toRoom)
            && space.IsAreaFree(elbowCell, fromRoom, toRoom);
        if (!pathIsFree) return null;

        return new List<CorridorPiece>
        {
            new CorridorPiece { area = horizontalLeg, runsAlongX = true },
            new CorridorPiece { area = elbowCell, isCorner = true, cornerRotationY = GetCornerRotation(opensEast, opensNorth) },
            new CorridorPiece { area = verticalLeg, runsAlongX = false },
        };
    }

    int GetCornerRotation(bool opensEast, bool opensNorth)
    {
        if (opensNorth && opensEast) return 0;
        if (opensEast) return 90;
        if (!opensNorth) return 180;
        return 270;
    }

    static float DistanceBetween(BSPNode roomA, BSPNode roomB) =>
        Vector2.Distance(roomA.roomBounds.center, roomB.roomBounds.center);
}