using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoomStyle", menuName = "Dungeon/Room Style")]
public class RoomStyleSO : ScriptableObject
{
    [Header("Génération")]
    [FormerlySerializedAs("Weight")] public float weight = 10;
    [FormerlySerializedAs("AllowAsStartRoom")] public bool canBeStartRoom;
    [FormerlySerializedAs("AllowAsEndRoom")] public bool canBeEndRoom;

    [Header("Salle")]
    [FormerlySerializedAs("roomFloorPrefab")] public GameObject floorPrefab;
    [FormerlySerializedAs("WallsPrefabsList")] public List<RoomWalls> wallPieces;
    [FormerlySerializedAs("FloorProps")] public List<GameObject> floorProps;

    [Header("Couloirs")]
    [FormerlySerializedAs("CorridorPrefab")] public GameObject corridorPrefab;
    [FormerlySerializedAs("CorridorCornerRightPrefab")] public GameObject corridorCornerPrefab;
    [FormerlySerializedAs("CorridorFloorPrefab")] public GameObject corridorFloorPrefab;
    public GameObject corridorEntrancePrefab;
}