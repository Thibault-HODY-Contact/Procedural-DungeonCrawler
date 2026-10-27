using System.Collections.Generic;
using UnityEngine;

public class RoomPropPlanner
{
    readonly BSPNode room;
    readonly PropRuleSO rule;
    readonly List<Vector3> prefabSizes;
    readonly RoomOccupancy occupancy;
    readonly System.Random rng;
    readonly Rect usableArea;

    public RoomPropPlanner(BSPNode room, PropRuleSO rule, List<Vector3> prefabSizes, RoomOccupancy occupancy, float wallInset, System.Random rng)
    {
        this.room = room;
        this.rule = rule;
        this.prefabSizes = prefabSizes;
        this.occupancy = occupancy;
        this.rng = rng;

        RectInt bounds = room.roomBounds;
        float margin = wallInset + rule.wallMargin;
        usableArea = Rect.MinMaxRect(bounds.xMin + margin, bounds.yMin + margin, bounds.xMax - margin, bounds.yMax - margin);
    }

    public List<PropPlacement> Plan()
    {
        int targetCount = rule.GetTargetCount(room, rng);
        List<Vector2> rankedSpots = rule.RankSpots(room, GetCandidateSpots(), rng);

        List<PropPlacement> placements = new List<PropPlacement>();
        foreach (Vector2 spot in rankedSpots)
        {
            if (placements.Count >= targetCount) break;

            int prefabIndex = rule.PickPrefabIndex(room, rng);
            if (prefabSizes[prefabIndex] == Vector3.zero) continue;

            float scale = rule.PickScale(room, placements.Count, targetCount, rng);
            if (TryPlace(spot, prefabIndex, scale, out PropPlacement placement))
                placements.Add(placement);
        }
        return placements;
    }

    List<Vector2> GetCandidateSpots()
    {
        List<Vector2> candidateSpots = new List<Vector2>();
        foreach (Vector2 spot in GetGridSpots())
            if (IsInWallBand(spot)) candidateSpots.Add(spot);
        return candidateSpots;
    }

    IEnumerable<Vector2> GetGridSpots()
    {
        float halfStep = rule.gridStep / 2f;
        for (float x = usableArea.xMin + halfStep; x < usableArea.xMax; x += rule.gridStep)
            for (float z = usableArea.yMin + halfStep; z < usableArea.yMax; z += rule.gridStep)
                yield return new Vector2(x, z);
    }

    bool TryPlace(Vector2 gridSpot, int prefabIndex, float scale, out PropPlacement placement)
    {
        placement = default;
        Vector3 size = prefabSizes[prefabIndex] * scale;
        float radius = Mathf.Max(size.x, size.z) / 2f;

        if (!TryKeepInside(gridSpot + GetRandomShift(), radius, out Vector2 spot)) return false;
        if (!occupancy.IsFree(spot, radius, rule.passageClearance, rule.spacing)) return false;

        occupancy.Take(spot, radius);
        placement = new PropPlacement
        {
            prefabIndex = prefabIndex,
            groundPoint = new Vector3(spot.x, room.floorHeight, spot.y),
            yaw = rule.randomRotation ? rng.Range(0f, 360f) : 0f,
            scale = scale,
        };
        return true;
    }

    Vector2 GetRandomShift()
    {
        float maxShift = rule.jitter * rule.gridStep / 2f;
        return new Vector2(rng.Range(-maxShift, maxShift), rng.Range(-maxShift, maxShift));
    }

    bool IsInWallBand(Vector2 spot)
    {
        if (!rule.onlyAlongWalls) return true;

        float distanceToWall = Mathf.Min(spot.x - usableArea.xMin, usableArea.xMax - spot.x, spot.y - usableArea.yMin, usableArea.yMax - spot.y);
        return distanceToWall <= rule.wallBandWidth;
    }

    bool TryKeepInside(Vector2 point, float radius, out Vector2 inside)
    {
        float x = Mathf.Clamp(point.x, usableArea.xMin + radius, usableArea.xMax - radius);
        float z = Mathf.Clamp(point.y, usableArea.yMin + radius, usableArea.yMax - radius);
        inside = new Vector2(x, z);
        return usableArea.width > radius * 2f && usableArea.height > radius * 2f;
    }
}