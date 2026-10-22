using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoomWalls", menuName = "Dungeon/RoomWalls")]
public class RoomWalls : ScriptableObject
{
    [FormerlySerializedAs("WallsPrefabs")] public GameObject prefab;
    [FormerlySerializedAs("Weight")] public float weight = 10;
}