using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropRule", menuName = "Dungeon/Prop Rule")]
public class PropRuleSO : ScriptableObject
{
    [Header("Props")]
    public List<GameObject> prefabs = new List<GameObject>();
    public bool randomRotation = true;
    [Range(0f, 1f)] public float sinkIntoGround = 0f;

    [Header("Taille")]
    [Min(0.01f)] public float minScale = 0.8f;
    [Min(0.01f)] public float maxScale = 1.1f;
    [Min(0.01f)] public float bigMinScale = 1.6f;
    [Min(0.01f)] public float bigMaxScale = 2.2f;
    [Range(0f, 1f)] public float bigChanceNearExit = 0.05f;
    [Range(0f, 1f)] public float bigChanceFarFromExit = 0.8f;

    [Header("Noise")]
    [Min(0.1f)] public float gridStep = 0.8f;
    [Min(0.001f)] public float noiseFrequency = 0.09f;
    [Range(0f, 1f)] public float jitter = 0.3f;
    [Range(0f, 1f)] public float thresholdNearExit = 0.82f;
    [Range(0f, 1f)] public float thresholdFarFromExit = 0.42f;
    public AnimationCurve distanceInfluence = AnimationCurve.Linear(0f, 0f, 1f, 1f);
   
    [Header("Guidage")]
    [Min(0)] public float exitDoorClearRadius = 6f;
    public bool pickPrefabByDistance = true;

    [Header("Placement")]
    public bool onlyAlongWalls = true;
    [Min(0)] public float wallBandWidth = 1.5f;
    [Min(0)] public float wallMargin = 0.1f;
    [Range(-1f, 2f)] public float spacing = -0.2f;
    [Min(0)] public float passageClearance = 2f;

    public float GetThreshold(float distanceToExit)
    {
        return Mathf.Lerp(thresholdNearExit, thresholdFarFromExit, distanceInfluence.Evaluate(distanceToExit));
    }

    public float GetBigChance(float distanceToExit)
    {
        return Mathf.Lerp(bigChanceNearExit, bigChanceFarFromExit, distanceInfluence.Evaluate(distanceToExit));
    }
}