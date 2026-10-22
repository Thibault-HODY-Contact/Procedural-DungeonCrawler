using System.Collections.Generic;
using UnityEngine;

public class CorridorBuilder
{
    const float HalfCell = 0.5f;

    readonly BSPMAPSO settings;
    readonly Transform parent;
    readonly int cornerRotationOffset;
    readonly float wallInset;

    public CorridorBuilder(BSPMAPSO settings, Transform parent, int cornerRotationOffset, float wallInset)
    {
        this.settings = settings;
        this.parent = parent;
        this.cornerRotationOffset = cornerRotationOffset;
        this.wallInset = wallInset;
    }

    public void SpawnAll(List<CorridorPiece> pieces, List<GameObject> spawnedObjects)
    {
        foreach (CorridorPiece piece in pieces)
        {
            RoomStyleSO style = PickRandomCorridorStyle();
            if (piece.isCorner) SpawnCorner(piece, style, spawnedObjects);
            else SpawnStraight(piece, pieces, style, spawnedObjects);
        }
    }

    RoomStyleSO PickRandomCorridorStyle()
    {
        return settings.corridorStyles[Random.Range(0, settings.corridorStyles.Count)];
    }

    void SpawnCorner(CorridorPiece piece, RoomStyleSO style, List<GameObject> spawnedObjects)
    {
        Vector3 center = GroundCenter(piece);
        Quaternion rotation = Quaternion.Euler(0f, piece.cornerRotationY + cornerRotationOffset, 0f);
        Vector3 size = new Vector3(settings.corridorWidth, MeshSize.KeepPrefabSize, settings.corridorWidth);

        SpawnResized(style.corridorCornerPrefab, center, rotation, size, spawnedObjects);
        SpawnResized(style.corridorFloorPrefab, center, rotation, size, spawnedObjects);
    }

    void SpawnStraight(CorridorPiece piece, List<CorridorPiece> pieces, RoomStyleSO style, List<GameObject> spawnedObjects)
    {
        Quaternion rotation = piece.runsAlongX ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.identity;
        Vector3 axis = rotation * Vector3.forward;
        Vector3 center = GroundCenter(piece);
        float length = piece.runsAlongX ? piece.area.width : piece.area.height;

        Vector3 floorSize = new Vector3(settings.corridorWidth, MeshSize.KeepPrefabSize, length);
        SpawnResized(style.corridorFloorPrefab, center, rotation, floorSize, spawnedObjects);

        bool startsAtRoom = !IsCornerAt(center - axis * (length / 2f + HalfCell), pieces);
        bool endsAtRoom = !IsCornerAt(center + axis * (length / 2f + HalfCell), pieces);
        Vector3 shellStart = center - axis * (length / 2f + (startsAtRoom ? wallInset : 0f));
        Vector3 shellEnd = center + axis * (length / 2f + (endsAtRoom ? wallInset : 0f));

        Vector3 shellSize = new Vector3(settings.corridorWidth, MeshSize.KeepPrefabSize, Vector3.Distance(shellStart, shellEnd));
        GameObject shell = SpawnResized(style.corridorPrefab, (shellStart + shellEnd) / 2f, rotation, shellSize, spawnedObjects);
        if (shell == null || style.corridorEntrancePrefab == null) return;

        Vector3 entranceLift = Vector3.up * (shell.transform.position.y - parent.position.y);
        if (startsAtRoom)
            spawnedObjects.Add(Object.Instantiate(style.corridorEntrancePrefab, shellStart + entranceLift, rotation, parent));
        if (endsAtRoom)
            spawnedObjects.Add(Object.Instantiate(style.corridorEntrancePrefab, shellEnd + entranceLift, rotation * Quaternion.Euler(0f, 180f, 0f), parent));
    }

    Vector3 GroundCenter(CorridorPiece piece)
    {
        return new Vector3(piece.area.center.x, parent.position.y, piece.area.center.y);
    }

    GameObject SpawnResized(GameObject prefab, Vector3 bottomCenter, Quaternion rotation, Vector3 size, List<GameObject> spawnedObjects)
    {
        if (prefab == null) return null;

        GameObject instance = Object.Instantiate(prefab, bottomCenter, rotation, parent);
        MeshSize.ResizeAndPlace(instance, MeshSize.GetLocalBounds(prefab), bottomCenter, size);
        spawnedObjects.Add(instance);
        return instance;
    }

    static bool IsCornerAt(Vector3 position, List<CorridorPiece> pieces)
    {
        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
        return pieces.Exists(other => other.isCorner && other.area.Contains(cell));
    }
}