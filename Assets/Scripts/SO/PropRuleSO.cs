using System.Collections.Generic;
using UnityEngine;

public abstract class PropRuleSO : ScriptableObject
{
    const float NoiseOffsetRange = 10000f;

    [Header("Props")]
    public List<GameObject> prefabs = new List<GameObject>();
    public bool randomRotation = true;
    [Range(0f, 1f)] public float sinkIntoGround = 0f;

    [Header("Taille")]
    [Min(0.01f)] public float minScale = 0.8f;
    [Min(0.01f)] public float maxScale = 1.1f;

    [Header("Noise")]
    [Min(0.1f)] public float gridStep = 0.8f;
    [Min(0.001f)] public float noiseFrequency = 0.09f;
    [Range(0f, 1f)] public float jitter = 0.3f;

    [Header("Placement")]
    public bool onlyAlongWalls = true;
    [Min(0)] public float wallBandWidth = 1.5f;
    [Min(0)] public float wallMargin = 0.1f;
    [Range(-1f, 2f)] public float spacing = -0.2f;
    [Min(0)] public float passageClearance = 2f;

    public abstract int GetTargetCount(BSPNode room, System.Random rng);

    public virtual bool CanSpawnIn(BSPNode room)
    {
        return true;
    }

    public virtual List<Vector2> RankSpots(BSPNode room, List<Vector2> spots, System.Random rng)
    {
        Vector2 noiseOffset = new Vector2(rng.Range(0f, NoiseOffsetRange), rng.Range(0f, NoiseOffsetRange));
        List<(Vector2 spot, float noise)> spotsWithNoise = spots.ConvertAll(spot => (spot, GetNoise(spot, noiseOffset)));
        spotsWithNoise.Sort((first, second) => second.noise.CompareTo(first.noise));
        return spotsWithNoise.ConvertAll(entry => entry.spot);
    }

    public virtual int PickPrefabIndex(BSPNode room, System.Random rng)
    {
        return rng.Next(prefabs.Count);
    }

    public virtual float PickScale(BSPNode room, int placedCount, int targetCount, System.Random rng)
    {
        return rng.Range(minScale, maxScale);
    }

    protected static float GetArea(BSPNode room)
    {
        return room.roomBounds.width * room.roomBounds.height;
    }

    float GetNoise(Vector2 spot, Vector2 noiseOffset)
    {
        return Mathf.PerlinNoise((spot.x + noiseOffset.x) * noiseFrequency, (spot.y + noiseOffset.y) * noiseFrequency);
    }
}