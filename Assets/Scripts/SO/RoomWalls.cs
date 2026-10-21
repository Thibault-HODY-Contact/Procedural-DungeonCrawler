using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName="RoomWalls", menuName = "Dungeon/RoomWalls")]
public class RoomWalls : ScriptableObject
{
    [Header("Identity")]
    public Color gizmoColor = Color.blue;

    [Header("Transform")]
    public float scaleX = 4.5f;
    public float scaleY = 4.5f;


    [Header("Generation")]
    public float Weight = 10;

    [Header("Prefab")]
    public GameObject WallsPrefabs;

   

   
}
