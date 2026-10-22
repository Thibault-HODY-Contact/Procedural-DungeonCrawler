using System.Collections.Generic;
using UnityEngine;

public class CorridorBuilder
{
    readonly BSPMAPSO settings;
    readonly Transform parent;
    readonly int cornerRotationOffset;

    public CorridorBuilder(BSPMAPSO settings, Transform parent, int cornerRotationOffset)
    {
        this.settings = settings;
        this.parent = parent;
        this.cornerRotationOffset = cornerRotationOffset;
    }

    public void SpawnAll(List<CorridorPiece> pieces, List<GameObject> spawnedObjects)
    {
        foreach (CorridorPiece piece in pieces)
        {
            RoomStyleSO style = PickRandomCorridorStyle();
            Vector3 bottomCenter = new Vector3(piece.area.center.x, parent.position.y, piece.area.center.y);
            Quaternion rotation = GetRotation(piece);
            Vector3 size = GetSize(piece);
            GameObject shellPrefab = piece.isCorner ? style.corridorCornerPrefab : style.corridorPrefab;

            SpawnResized(shellPrefab, bottomCenter, rotation, size, spawnedObjects);
            SpawnResized(style.corridorFloorPrefab, bottomCenter, rotation, size, spawnedObjects);
        }
    }

    RoomStyleSO PickRandomCorridorStyle()
    {
        return settings.corridorStyles[Random.Range(0, settings.corridorStyles.Count)];
    }

    Quaternion GetRotation(CorridorPiece piece)
    {
        if (piece.isCorner) return Quaternion.Euler(0f, piece.cornerRotationY + cornerRotationOffset, 0f);
        return piece.runsAlongX ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.identity;
    }

    Vector3 GetSize(CorridorPiece piece)
    {
        int width = settings.corridorWidth;
        int length = piece.isCorner ? width : piece.runsAlongX ? piece.area.width : piece.area.height;
        return new Vector3(width, MeshSize.KeepPrefabSize, length);
    }

    void SpawnResized(GameObject prefab, Vector3 bottomCenter, Quaternion rotation, Vector3 size, List<GameObject> spawnedObjects)
    {
        if (prefab == null) return;

        GameObject instance = Object.Instantiate(prefab, bottomCenter, rotation, parent);
        MeshSize.ResizeAndPlace(instance, MeshSize.GetLocalBounds(prefab), bottomCenter, size);
        spawnedObjects.Add(instance);
    }
}