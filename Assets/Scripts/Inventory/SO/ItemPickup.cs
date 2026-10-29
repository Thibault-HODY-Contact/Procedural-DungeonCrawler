using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] ItemSO _item;
    [SerializeField, Min(1)] int _amount = 1;

    void OnTriggerEnter(Collider other)
    {
        Inventory inventory = other.GetComponentInParent<Inventory>();
        if (inventory == null || _amount <= 0) return;

        _amount -= inventory.Add(_item, _amount);
        if (_amount <= 0) Destroy(gameObject);
    }
}