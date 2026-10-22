using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder
{
    const float Tolerance = 0.01f;

    struct WallOption
    {
        public GameObject prefab;
        public float width;
        public float weight;
    }

    readonly Transform parent;
    readonly List<CorridorPiece> corridors;
    readonly float wallInset;

    public RoomBuilder(Transform parent, List<CorridorPiece> corridors, float wallInset)
    {
        this.parent = parent;
        this.corridors = corridors;
        this.wallInset = wallInset;
    }

    public void BuildFloors(List<BSPNode> rooms, List<GameObject> spawnedFloors)
    {
        foreach (BSPNode room in rooms)
        {
            RectInt bounds = room.roomBounds;
            GameObject prefab = room.style.floorPrefab;
            Vector3 bottomCenter = new Vector3(bounds.center.x, room.floorHeight, bounds.center.y);
            Vector3 size = new Vector3(bounds.width, MeshSize.KeepPrefabSize, bounds.height);

            GameObject floor = Object.Instantiate(prefab, bottomCenter, Quaternion.identity, parent);
            MeshSize.ResizeAndPlace(floor, MeshSize.GetLocalBounds(prefab), bottomCenter, size);
            spawnedFloors.Add(floor);
        }
    }

    public void BuildWalls(List<BSPNode> rooms, List<GameObject> spawnedWalls)
    {
        float groundY = parent.position.y;
        foreach (BSPNode room in rooms)
        {
            RectInt bounds = room.roomBounds;

            BuildSideWithDoors(room, bounds.xMin, bounds.xMax, x => new Vector2Int(x, bounds.yMax),
                x => new Vector3(x, groundY, bounds.yMax - wallInset), Vector3.right, 0f, spawnedWalls);
            BuildSideWithDoors(room, bounds.yMin, bounds.yMax, z => new Vector2Int(bounds.xMax, z),
                z => new Vector3(bounds.xMax - wallInset, groundY, z), Vector3.forward, 90f, spawnedWalls);
            BuildSideWithDoors(room, bounds.xMin, bounds.xMax, x => new Vector2Int(x, bounds.yMin - 1),
                x => new Vector3(x, groundY, bounds.yMin + wallInset), Vector3.right, 180f, spawnedWalls);
            BuildSideWithDoors(room, bounds.yMin, bounds.yMax, z => new Vector2Int(bounds.xMin - 1, z),
                z => new Vector3(bounds.xMin + wallInset, groundY, z), Vector3.forward, 270f, spawnedWalls);
        }
    }

    void BuildSideWithDoors(BSPNode room, int firstCell, int endCell,
        System.Func<int, Vector2Int> cellOutsideRoom, System.Func<float, Vector3> positionOnSide,
        Vector3 direction, float yRotation, List<GameObject> spawnedWalls)
    {
        int wallStart = firstCell;
        for (int cell = firstCell; cell <= endCell; cell++)
        {
            bool reachedEnd = cell == endCell;
            bool isDoor = !reachedEnd && IsCorridor(cellOutsideRoom(cell));
            if (!isDoor && !reachedEnd) continue;

            if (cell > wallStart)
                FillWallSegment(room, cell - wallStart, positionOnSide(wallStart), direction, yRotation, spawnedWalls);
            wallStart = cell + 1;
        }
    }

    void FillWallSegment(BSPNode room, float length, Vector3 start, Vector3 direction, float yRotation, List<GameObject> spawnedWalls)
    {
        List<WallOption> options = GetWallOptions(room.style);
        if (options.Count == 0) return;

        WallOption narrowest = options[0];
        Quaternion rotation = Quaternion.Euler(0f, yRotation, 0f);
        float filledLength = 0f;

        while (length - filledLength >= narrowest.width - Tolerance)
        {
            WallOption chosen = PickWidestFitting(options, length - filledLength);
            Vector3 bottomCenter = start + direction * (filledLength + chosen.width / 2f);
            spawnedWalls.Add(SpawnWall(chosen.prefab, bottomCenter, rotation, MeshSize.KeepPrefabSize));
            filledLength += chosen.width;
        }

        float leftover = length - filledLength;
        if (leftover > Tolerance)
        {
            Vector3 bottomCenter = start + direction * (filledLength + leftover / 2f);
            spawnedWalls.Add(SpawnWall(narrowest.prefab, bottomCenter, rotation, leftover));
        }
    }

    List<WallOption> GetWallOptions(RoomStyleSO style)
    {
        List<WallOption> options = new List<WallOption>();
        foreach (RoomWalls wall in style.wallPieces)
        {
            if (wall == null || wall.prefab == null) continue;

            float width = MeshSize.GetScaledSize(wall.prefab).x;
            if (width > Tolerance)
                options.Add(new WallOption { prefab = wall.prefab, width = width, weight = wall.weight });
        }
        options.Sort((first, second) => first.width.CompareTo(second.width));
        return options;
    }

    WallOption PickWidestFitting(List<WallOption> options, float remainingLength)
    {
        float widestWidth = 0f;
        foreach (WallOption option in options)
            if (option.width <= remainingLength + Tolerance)
                widestWidth = Mathf.Max(widestWidth, option.width);

        return PickByWeight(options.FindAll(option => Mathf.Approximately(option.width, widestWidth)));
    }

    WallOption PickByWeight(List<WallOption> candidates)
    {
        float totalWeight = 0f;
        foreach (WallOption candidate in candidates)
            totalWeight += Mathf.Max(0f, candidate.weight);
        if (totalWeight <= 0f) return candidates[Random.Range(0, candidates.Count)];

        float roll = Random.Range(0f, totalWeight);
        foreach (WallOption candidate in candidates)
        {
            roll -= Mathf.Max(0f, candidate.weight);
            if (roll <= 0f) return candidate;
        }
        return candidates[candidates.Count - 1];
    }

    GameObject SpawnWall(GameObject prefab, Vector3 bottomCenter, Quaternion rotation, float width)
    {
        GameObject wall = Object.Instantiate(prefab, bottomCenter, rotation, parent);
        Vector3 size = new Vector3(width, MeshSize.KeepPrefabSize, MeshSize.KeepPrefabSize);
        MeshSize.ResizeAndPlace(wall, MeshSize.GetLocalBounds(prefab), bottomCenter, size);
        return wall;
    }

    bool IsCorridor(Vector2Int cell)
    {
        foreach (CorridorPiece piece in corridors)
            if (piece.area.Contains(cell)) return true;
        return false;
    }
}