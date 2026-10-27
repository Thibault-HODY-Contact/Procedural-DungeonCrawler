using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OreRule", menuName = "Dungeon/Prop Rules/Ore Rule")]
public class OreRuleSO : PropRuleSO
{
    [Header("Gros minerais")]
    [Min(0.01f)] public float bigMinScale = 1.6f;
    [Min(0.01f)] public float bigMaxScale = 2.2f;
    [Range(0f, 1f)] public float bigChanceNearExit = 0.05f;
    [Range(0f, 1f)] public float bigChanceFarFromExit = 0.8f;

    [Header("Quantité par 100 m²")]
    [Min(0)] public float countNearExit = 0f;
    [Min(0)] public float countFarFromExit = 10f;
    public AnimationCurve distanceInfluence = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Guidage")]
    [Min(0)] public float exitDoorClearRadius = 6f;
    public bool pickPrefabByDistance = true;

    public override int GetTargetCount(BSPNode room, System.Random rng)
    {
        float countPerHundredSquareMeters = Mathf.Lerp(countNearExit, countFarFromExit, GetDistanceWeight(room));
        return Mathf.RoundToInt(countPerHundredSquareMeters * GetArea(room) / 100f);
    }

    public override List<Vector2> RankSpots(BSPNode room, List<Vector2> spots, System.Random rng)
    {
        List<Vector2> openSpots = spots.FindAll(spot => !IsNearExitDoor(room, spot));
        List<Vector2> spotsNearExitDoor = spots.FindAll(spot => IsNearExitDoor(room, spot));
        spotsNearExitDoor.Sort((first, second) => GetDistanceToExitDoor(room, second).CompareTo(GetDistanceToExitDoor(room, first)));

        List<Vector2> rankedSpots = base.RankSpots(room, openSpots, rng);
        rankedSpots.AddRange(spotsNearExitDoor);
        return rankedSpots;
    }

    public override int PickPrefabIndex(BSPNode room, System.Random rng)
    {
        if (!pickPrefabByDistance) return base.PickPrefabIndex(room, rng);

        int indexByDistance = Mathf.FloorToInt(room.distanceToExit * prefabs.Count);
        return Mathf.Clamp(indexByDistance, 0, prefabs.Count - 1);
    }

    public override float PickScale(BSPNode room, int placedCount, int targetCount, System.Random rng)
    {
        float bigChance = Mathf.Lerp(bigChanceNearExit, bigChanceFarFromExit, GetDistanceWeight(room));
        int bigCount = Mathf.RoundToInt(bigChance * targetCount);
        if (placedCount >= bigCount) return base.PickScale(room, placedCount, targetCount, rng);

        return rng.Range(bigMinScale, bigMaxScale);
    }

    float GetDistanceWeight(BSPNode room)
    {
        return distanceInfluence.Evaluate(room.distanceToExit);
    }

    bool IsNearExitDoor(BSPNode room, Vector2 spot)
    {
        return GetDistanceToExitDoor(room, spot) < exitDoorClearRadius;
    }

    float GetDistanceToExitDoor(BSPNode room, Vector2 spot)
    {
        return Vector2.Distance(spot, room.doorTowardExit);
    }
}