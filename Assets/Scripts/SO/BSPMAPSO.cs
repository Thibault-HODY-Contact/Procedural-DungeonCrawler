using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BSPMAPSO", menuName = "Dungeon/BSPMAPSO")]
public class BSPMAPSO : ScriptableObject
{
    [Header("Carte")]
    public Vector2Int MapSize = new Vector2Int(100, 100);
    public Color gizmoColor = Color.white;

    [Header("Découpage BSP")]
    [Min(1)] public int MaxDepth = 4;       
    [Min(4)] public int MinNodeSize = 16;   

    [Header("Salles")]
    [Min(0)] public int Padding = 2;        
    public Vector2Int MinRoomSize = new Vector2Int(6, 6);
    public List<RoomStyleSO> RoomStyles;

    [Header("Couloirs")]
    public RoomStyleSO CorridorStyle;
    [Min(1)] public int CorridorWidth = 3;

    public RectInt MapBounds ;


}