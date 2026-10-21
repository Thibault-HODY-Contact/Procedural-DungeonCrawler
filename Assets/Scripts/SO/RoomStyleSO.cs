using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName="RoomStyle", menuName = "Dungeon/Room Style")]
public class RoomStyleSO : ScriptableObject
{
    [Header("Identity")]
    public string styleId = "Normal";
    public Color gizmoColor = Color.white;

    [Header("Transform")]
     public int minX= 6;
     public int minY = 6;
     public int maxX = 12;
     public int maxY  = 12;
     public Vector2 center;


    [Header("Generation")]
    public float Weight = 10;
    public bool AllowAsStartRoom;
    public bool AllowAsEndRoom;

    [Header("Prefabs")]
    public GameObject roomFloorPrefab;
    public GameObject CorridorFloorPrefab;
    public GameObject CorridorPrefab;
    public GameObject WallPrefab;
    public GameObject CorridorCornerRightPrefab;
    public GameObject CorridorCornerLeftPrefab;

    [Header("Props")]
    public List<GameObject> FloorProps;
    private RoomStyleSO roomStyleSO;

    public RoomStyleSO(RoomStyleSO roomStyleSO)
    {
        this.roomStyleSO = roomStyleSO;
    }
}
