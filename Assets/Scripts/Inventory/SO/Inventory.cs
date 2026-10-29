using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int SlotCount = 2;

    readonly InventorySlot[] _slots = new InventorySlot[SlotCount];

    public int SelectedIndex { get; private set; }
    public event Action OnChanged;

    void Awake()
    {
        for (int i = 0; i < SlotCount; i++)
            _slots[i] = new InventorySlot();
    }

    public InventorySlot GetSlot(int index)
    {
        return _slots[index];
    }

    public int Add(ItemSO item, int amount)
    {
        if (item == null) return 0;

        InventorySlot slot = FindSlotFor(item);
        if (slot == null) return 0;

        int added = Mathf.Min(amount, item.maxStack - slot.count);
        if (added <= 0) return 0;

        slot.item = item;
        slot.count += added;
        OnChanged?.Invoke();
        return added;
    }

    public void Select(int index)
    {
        SelectedIndex = Mathf.Clamp(index, 0, SlotCount - 1);
        OnChanged?.Invoke();
    }

    public void UseSelected()
    {
        InventorySlot slot = _slots[SelectedIndex];
        if (slot.IsEmpty || !slot.item.Use(gameObject)) return;

        slot.count--;
        if (slot.count == 0) slot.item = null;
        OnChanged?.Invoke();
    }

    InventorySlot FindSlotFor(ItemSO item)
    {
        foreach (InventorySlot slot in _slots)
            if (slot.item == item) return slot;

        foreach (InventorySlot slot in _slots)
            if (slot.IsEmpty) return slot;

        return null;
    }
}