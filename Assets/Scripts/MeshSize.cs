using UnityEngine;

public static class MeshSize
{
    public const float KeepPrefabSize = 0f;
    const float MinMeshLength = 0.0001f;

    public static Bounds GetLocalBounds(GameObject prefab)
    {
        Matrix4x4 worldToRoot = prefab.transform.worldToLocalMatrix;
        Bounds localBounds = new Bounds();
        bool hasPoint = false;

        foreach (MeshFilter meshFilter in GetMeasuredMeshes(prefab))
        {
            if (meshFilter.sharedMesh == null) continue;

            Matrix4x4 meshToRoot = worldToRoot * meshFilter.transform.localToWorldMatrix;
            foreach (Vector3 corner in GetCorners(meshFilter.sharedMesh.bounds))
            {
                Vector3 point = meshToRoot.MultiplyPoint3x4(corner);
                if (hasPoint) localBounds.Encapsulate(point);
                else localBounds = new Bounds(point, Vector3.zero);
                hasPoint = true;
            }
        }
        return localBounds;
    }

    public static Vector3 GetScaledSize(GameObject prefab)
    {
        return Vector3.Scale(GetLocalBounds(prefab).size, prefab.transform.localScale);
    }

    public static void ResizeAndPlace(GameObject instance, Bounds localBounds, Vector3 bottomCenter, Vector3 targetSize)
    {
        Vector3 scale = instance.transform.localScale;
        scale.x = ScaleForLength(scale.x, localBounds.size.x, targetSize.x);
        scale.y = ScaleForLength(scale.y, localBounds.size.y, targetSize.y);
        scale.z = ScaleForLength(scale.z, localBounds.size.z, targetSize.z);
        instance.transform.localScale = scale;

        PlaceOnGround(instance, localBounds, bottomCenter);
    }

    public static void PlaceOnGround(GameObject instance, Bounds localBounds, Vector3 bottomCenter)
    {
        Vector3 pivotToBottomCenter = new Vector3(localBounds.center.x, localBounds.min.y, localBounds.center.z);
        Vector3 scaledOffset = Vector3.Scale(pivotToBottomCenter, instance.transform.localScale);
        instance.transform.position = bottomCenter - instance.transform.rotation * scaledOffset;
    }

    static MeshFilter[] GetMeasuredMeshes(GameObject prefab)
    {
        PieceSize pieceSize = prefab.GetComponent<PieceSize>();
        if (pieceSize != null && pieceSize.referenceMesh != null)
            return new[] { pieceSize.referenceMesh };
        return prefab.GetComponentsInChildren<MeshFilter>(true);
    }

    static Vector3[] GetCorners(Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        return new[]
        {
            new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z),
            new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z),
            new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z),
            new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z),
        };
    }

    static float ScaleForLength(float currentScale, float meshLength, float targetLength)
    {
        bool keepCurrentScale = targetLength <= KeepPrefabSize || meshLength < MinMeshLength;
        return keepCurrentScale ? currentScale : targetLength / meshLength;
    }
}