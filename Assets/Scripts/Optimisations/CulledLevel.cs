using System.Collections.Generic;
using UnityEngine;
class CulledLevel
{
    public DungeonGeneratorBSP generator;
    public float bottom;
    public float top;
    public readonly List<(Renderer renderer, Bounds bounds)> renderers = new List<(Renderer renderer, Bounds bounds)>();
    public readonly List<Light> lights = new List<Light>();
}