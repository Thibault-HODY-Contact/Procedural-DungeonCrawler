using UnityEngine;

[CreateAssetMenu(fileName = "ScatterRule", menuName = "Dungeon/Prop Rules/Scatter Rule")]
public class ScatterRuleSO : PropRuleSO
{
    [Header("Quantité par 100 m²")]
    [Min(0)] public float minCount = 1f;
    [Min(0)] public float maxCount = 3f;

    [Header("Salles")]
    [Min(0)] public float minRoomArea = 0f;

    public override bool CanSpawnIn(BSPNode room)
    {
        return GetArea(room) >= minRoomArea;
    }

    public override int GetTargetCount(BSPNode room, System.Random rng)
    {
        float countPerHundredSquareMeters = rng.Range(minCount, maxCount);
        return Mathf.RoundToInt(countPerHundredSquareMeters * GetArea(room) / 100f);
    }
}