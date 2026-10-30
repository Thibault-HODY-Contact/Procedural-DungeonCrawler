using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    public Sprite icon;
    [Min(1)] public int maxStack = 3;

    public abstract bool Use(GameObject user);
}