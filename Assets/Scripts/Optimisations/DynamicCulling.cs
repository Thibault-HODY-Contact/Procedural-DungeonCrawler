using System.Collections.Generic;
using UnityEngine;

public class DynamicCulling : MonoBehaviour
{
    [SerializeField] float viewDistance = 40f;
    [SerializeField] float viewHeight = 3f;
    [SerializeField] float refreshInterval = 0.2f;

    readonly List<CulledLevel> levels = new List<CulledLevel>();
    float nextRefreshTime;

    void Update()
    {
        if (Time.time < nextRefreshTime) return;
        nextRefreshTime = Time.time + refreshInterval;

        if (levels.Count == 0 || levels.Exists(level => level.generator == null)) CollectLevels();
        foreach (CulledLevel level in levels) Refresh(level);
    }

    void OnDisable()
    {
        foreach (CulledLevel level in levels) ShowAll(level);
    }

    void CollectLevels()
    {
        levels.Clear();
        foreach (DungeonGeneratorBSP generator in FindObjectsByType<DungeonGeneratorBSP>(FindObjectsSortMode.None))
            levels.Add(CollectLevel(generator));
    }

    CulledLevel CollectLevel(DungeonGeneratorBSP generator)
    {
        float levelHeight = generator.transform.position.y;
        CulledLevel level = new CulledLevel { generator = generator, bottom = levelHeight, top = levelHeight };

        foreach (Renderer renderer in generator.GetComponentsInChildren<Renderer>())
        {
            if (!renderer.enabled) continue;

            Bounds bounds = renderer.bounds;
            level.renderers.Add((renderer, bounds));
            level.bottom = Mathf.Min(level.bottom, bounds.min.y);
            level.top = Mathf.Max(level.top, bounds.max.y);
        }

        foreach (Light light in generator.GetComponentsInChildren<Light>())
            if (light.enabled) level.lights.Add(light);

        return level;
    }

    void Refresh(CulledLevel level)
    {
        bool levelInView = IsLevelInView(level);

        foreach ((Renderer renderer, Bounds bounds) in level.renderers)
            if (renderer != null) renderer.enabled = levelInView && IsNear(bounds.ClosestPoint(transform.position));

        foreach (Light light in level.lights)
            if (light != null) light.enabled = levelInView && IsNear(light.transform.position);
    }

    void ShowAll(CulledLevel level)
    {
        foreach ((Renderer renderer, Bounds bounds) in level.renderers)
            if (renderer != null) renderer.enabled = true;

        foreach (Light light in level.lights)
            if (light != null) light.enabled = true;
    }

    bool IsLevelInView(CulledLevel level)
    {
        float height = transform.position.y;
        return height > level.bottom - viewHeight && height < level.top + viewHeight;
    }

    bool IsNear(Vector3 point)
    {
        Vector3 viewer = transform.position;
        return Vector2.Distance(new Vector2(point.x, point.z), new Vector2(viewer.x, viewer.z)) <= viewDistance;
    }
}