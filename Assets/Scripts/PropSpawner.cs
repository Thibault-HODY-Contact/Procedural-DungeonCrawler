using System.Collections.Generic;
using UnityEngine;

public class PropSpawner
{
    const float NoiseOffsetRange = 10000f;

    struct PlacedProp
    {
        public Vector2 position;
        public float radius;
    }

    readonly Transform parent;
    readonly List<CorridorPiece> corridors;
    readonly float wallInset;
    readonly System.Random rng;

    public PropSpawner(Transform parent, List<CorridorPiece> corridors, float wallInset, int seed)
    {
        this.parent = parent;
        this.corridors = corridors;
        this.wallInset = wallInset;
        rng = new System.Random(seed);
    }

    public void SpawnAll(List<BSPNode> rooms, List<Vector2> reservedSpots, List<GameObject> spawnedProps)
    {
        foreach (BSPNode room in rooms)
        {
            if (room.style.propRules == null) continue;

            List<PlacedProp> placed = new List<PlacedProp>();
            foreach (PropRuleSO rule in room.style.propRules)
                if (rule != null && rule.prefabs.Count > 0)
                    SpawnRule(room, rule, reservedSpots, placed, spawnedProps);
        }
    }

    void SpawnRule(BSPNode room, PropRuleSO rule, List<Vector2> reservedSpots, List<PlacedProp> placed, List<GameObject> spawnedProps)
    {
        Rect area = GetUsableArea(room.roomBounds, wallInset + rule.wallMargin);
        Vector2 noiseOffset = new Vector2(RandomRange(0f, NoiseOffsetRange), RandomRange(0f, NoiseOffsetRange));

        foreach (List<Vector2> vein in FindVeins(room, area, rule, noiseOffset))
        {
            GameObject prefab = PickPrefab(rule, room.distanceToExit);
            if (prefab == null) continue;

            foreach (Vector2 cellCenter in vein)
                TrySpawnAt(prefab, cellCenter, room, rule, area, reservedSpots, placed, spawnedProps);
        }
    }

    List<List<Vector2>> FindVeins(BSPNode room, Rect area, PropRuleSO rule, Vector2 noiseOffset)
    {
        int columns = Mathf.Max(0, Mathf.FloorToInt(area.width / rule.gridStep));
        int rows = Mathf.Max(0, Mathf.FloorToInt(area.height / rule.gridStep));
        bool[,] hasOre = new bool[columns, rows];

        for (int column = 0; column < columns; column++)
            for (int row = 0; row < rows; row++)
            {
                Vector2 cell = GetCellCenter(area, rule.gridStep, column, row);
                float noise = Mathf.PerlinNoise((cell.x + noiseOffset.x) * rule.noiseFrequency, (cell.y + noiseOffset.y) * rule.noiseFrequency);
                float threshold = rule.GetThreshold(GetDistanceToExit(room, cell, rule));
                hasOre[column, row] = noise >= threshold && IsInWallBand(cell, area, rule);
            }

        List<List<Vector2>> veins = new List<List<Vector2>>();
        bool[,] visited = new bool[columns, rows];
        for (int column = 0; column < columns; column++)
            for (int row = 0; row < rows; row++)
                if (hasOre[column, row] && !visited[column, row])
                    veins.Add(FloodFillVein(new Vector2Int(column, row), hasOre, visited, area, rule.gridStep));
        return veins;
    }

    List<Vector2> FloodFillVein(Vector2Int start, bool[,] hasOre, bool[,] visited, Rect area, float gridStep)
    {
        List<Vector2> vein = new List<Vector2>();
        Stack<Vector2Int> toVisit = new Stack<Vector2Int>();
        toVisit.Push(start);
        visited[start.x, start.y] = true;

        while (toVisit.Count > 0)
        {
            Vector2Int cell = toVisit.Pop();
            vein.Add(GetCellCenter(area, gridStep, cell.x, cell.y));

            foreach (Vector2Int next in new[] { cell + Vector2Int.right, cell + Vector2Int.left, cell + Vector2Int.up, cell + Vector2Int.down })
            {
                bool insideGrid = next.x >= 0 && next.y >= 0 && next.x < hasOre.GetLength(0) && next.y < hasOre.GetLength(1);
                if (!insideGrid || visited[next.x, next.y] || !hasOre[next.x, next.y]) continue;
                visited[next.x, next.y] = true;
                toVisit.Push(next);
            }
        }
        return vein;
    }

    void TrySpawnAt(GameObject prefab, Vector2 cellCenter, BSPNode room, PropRuleSO rule, Rect area, List<Vector2> reservedSpots, List<PlacedProp> placed, List<GameObject> spawnedProps)
    {
        float scale = PickScale(rule, GetDistanceToExit(room, cellCenter, rule));
        Vector3 size = MeshSize.GetScaledSize(prefab) * scale;
        float radius = Mathf.Max(size.x, size.z) / 2f;

        float maxShift = rule.jitter * rule.gridStep / 2f;
        Vector2 shifted = cellCenter + new Vector2(RandomRange(-maxShift, maxShift), RandomRange(-maxShift, maxShift));
        if (!TryKeepInside(shifted, area, radius, out Vector2 spot)) return;
        if (!IsFree(spot, radius, rule, reservedSpots, placed)) return;

        placed.Add(new PlacedProp { position = spot, radius = radius });
        spawnedProps.Add(SpawnProp(prefab, rule, scale, new Vector3(spot.x, room.floorHeight, spot.y)));
    }

    bool IsFree(Vector2 spot, float radius, PropRuleSO rule, List<Vector2> reservedSpots, List<PlacedProp> placed)
    {
        foreach (CorridorPiece corridor in corridors)
            if (Grow(corridor.area, rule.passageClearance + radius).Contains(spot)) return false;

        foreach (Vector2 reserved in reservedSpots)
            if (Vector2.Distance(reserved, spot) < rule.passageClearance + radius) return false;

        foreach (PlacedProp other in placed)
            if (Vector2.Distance(other.position, spot) < other.radius + radius + rule.spacing) return false;

        return true;
    }

    GameObject SpawnProp(GameObject prefab, PropRuleSO rule, float scale, Vector3 groundPoint)
    {
        float yaw = rule.randomRotation ? RandomRange(0f, 360f) : 0f;
        GameObject prop = Object.Instantiate(prefab, groundPoint, Quaternion.Euler(0f, yaw, 0f), parent);
        prop.transform.localScale *= scale;
        Bounds bounds = MeshSize.GetLocalBounds(prefab);
        float sinkDepth = bounds.size.y * prop.transform.localScale.y * rule.sinkIntoGround;
        MeshSize.PlaceOnGround(prop, bounds, groundPoint + Vector3.down * sinkDepth);
        return prop;
    }
    float PickScale(PropRuleSO rule, float distanceToExit)
    {
        bool isBig = rng.NextDouble() < rule.GetBigChance(distanceToExit);
        return isBig ? RandomRange(rule.bigMinScale, rule.bigMaxScale) : RandomRange(rule.minScale, rule.maxScale);
    }

    float RandomRange(float min, float max)
    {
        return Mathf.Lerp(min, max, (float)rng.NextDouble());
    }
    GameObject PickPrefab(PropRuleSO rule, float distanceToExit)
    {
        if (!rule.pickPrefabByDistance) return rule.prefabs[rng.Next(rule.prefabs.Count)];
        int index = Mathf.FloorToInt(distanceToExit * rule.prefabs.Count);
        return rule.prefabs[Mathf.Clamp(index, 0, rule.prefabs.Count - 1)];
    }

    static float GetDistanceToExit(BSPNode room, Vector2 point, PropRuleSO rule)
    {
        bool nearExitDoor = Vector2.Distance(point, room.doorTowardExit) < rule.exitDoorClearRadius;
        return nearExitDoor ? 0f : room.distanceToExit;
    }

    static bool IsInWallBand(Vector2 point, Rect area, PropRuleSO rule)
    {
        if (!rule.onlyAlongWalls) return true;
        float distanceToWall = Mathf.Min(point.x - area.xMin, area.xMax - point.x, point.y - area.yMin, area.yMax - point.y);
        return distanceToWall <= rule.wallBandWidth;
    }

    static bool TryKeepInside(Vector2 point, Rect area, float radius, out Vector2 inside)
    {
        inside = new Vector2(Mathf.Clamp(point.x, area.xMin + radius, area.xMax - radius), Mathf.Clamp(point.y, area.yMin + radius, area.yMax - radius));
        return area.width > radius * 2f && area.height > radius * 2f;
    }

    static Vector2 GetCellCenter(Rect area, float gridStep, int column, int row)
    {
        return new Vector2(area.xMin + (column + 0.5f) * gridStep, area.yMin + (row + 0.5f) * gridStep);
    }

    static Rect GetUsableArea(RectInt bounds, float margin)
    {
        return Rect.MinMaxRect(bounds.xMin + margin, bounds.yMin + margin, bounds.xMax - margin, bounds.yMax - margin);
    }

    static Rect Grow(RectInt area, float amount)
    {
        return new Rect(area.x - amount, area.y - amount, area.width + amount * 2f, area.height + amount * 2f);
    }
}