using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BSPMAPSO", menuName = "Dungeon/BSPMAPSO")]
public class BSPMAPSO : ScriptableObject
{
    [Header("Carte")]
    [FormerlySerializedAs("MapSize")] public Vector2Int mapSize = new Vector2Int(100, 100);

    [Header("Découpage BSP")]
    [FormerlySerializedAs("MaxDepth")][Min(1)] public int maxSplitDepth = 4;
    [FormerlySerializedAs("MinNodeSize")][Min(4)] public int minLeafSize = 16;

    [Header("Salles")]
    [FormerlySerializedAs("Padding")][Min(0)] public int roomPadding = 2;
    [FormerlySerializedAs("MinRoomSize")] public Vector2Int minRoomSize = new Vector2Int(6, 6);
    [FormerlySerializedAs("RoomStyles")] public List<RoomStyleSO> roomStyles;

    [Header("Couloirs")]
    [FormerlySerializedAs("CorridorStyle")] public List<RoomStyleSO> corridorStyles;
    [FormerlySerializedAs("CorridorPadding")] public int corridorMargin = 2;
    [FormerlySerializedAs("CorridorWidth")][Min(1)] public int corridorWidth = 1;
}