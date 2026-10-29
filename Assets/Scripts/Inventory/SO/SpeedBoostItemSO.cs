using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostItem", menuName = "Inventory/Speed Boost Item")]
public class SpeedBoostItemSO : ItemSO
{
    [Min(1f)] public float speedMultiplier = 1.5f;
    [Min(0.1f)] public float duration = 5f;

    public override bool Use(GameObject user)
    {
        MoveScript move = user.GetComponentInChildren<MoveScript>();
        if (move == null) return false;

        move.BoostSpeed(speedMultiplier, duration);
        return true;
    }
}