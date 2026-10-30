using UnityEngine;

public static class RandomExtensions
{
    public static float Range(this System.Random rng, float min, float max)
    {
        return Mathf.Lerp(min, max, (float)rng.NextDouble());
    }
}