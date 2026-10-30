using System.Collections.Generic;
using UnityEngine;

public class PropSpawner
{
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

            RoomOccupancy occupancy = new RoomOccupancy(corridors, reservedSpots);
            foreach (PropRuleSO rule in room.style.propRules)
            {
                if (rule == null || rule.prefabs.Count == 0 || !rule.CanSpawnIn(room)) continue;

                List<Vector3> prefabSizes = rule.prefabs.ConvertAll(MeasurePrefab);
                RoomPropPlanner planner = new RoomPropPlanner(room, rule, prefabSizes, occupancy, wallInset, rng);
                foreach (PropPlacement placement in planner.Plan())
                    spawnedProps.Add(Spawn(rule.prefabs[placement.prefabIndex], placement, rule.sinkIntoGround));
            }
        }
    }

    GameObject Spawn(GameObject prefab, PropPlacement placement, float sinkIntoGround)
    {
        GameObject prop = Object.Instantiate(prefab, placement.groundPoint, Quaternion.Euler(0f, placement.yaw, 0f), parent);
        prop.transform.localScale *= placement.scale;

        Bounds bounds = MeshSize.GetLocalBounds(prefab);
        float sinkDepth = bounds.size.y * prop.transform.localScale.y * sinkIntoGround;
        MeshSize.PlaceOnGround(prop, bounds, placement.groundPoint + Vector3.down * sinkDepth);
        return prop;
    }

    static Vector3 MeasurePrefab(GameObject prefab)
    {
        return prefab == null ? Vector3.zero : MeshSize.GetScaledSize(prefab);
    }
}