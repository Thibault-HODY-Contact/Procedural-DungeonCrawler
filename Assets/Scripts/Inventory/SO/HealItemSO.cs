using UnityEngine;

[CreateAssetMenu(fileName = "HealItem", menuName = "Inventory/Heal Item")]
public class HealItemSO : ItemSO
{
    [Min(1)] public int healAmount = 10;

    public override bool Use(GameObject user)
    {
        LifeScript life = user.GetComponentInChildren<LifeScript>();
        return life != null && life.Heal(healAmount);
    }
}