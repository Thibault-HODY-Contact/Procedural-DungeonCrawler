using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Serializable]
    public class SlotView
    {
        public Image icon;
        public TMP_Text count;
        public GameObject selectionFrame;
    }

    [SerializeField] Inventory _inventory;
    [SerializeField] List<SlotView> _slotViews;

    void Start()
    {
        _inventory.OnChanged += UpdateUI;
        UpdateUI();
    }

    void OnDestroy()
    {
        _inventory.OnChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        for (int i = 0; i < _slotViews.Count; i++)
        {
            InventorySlot slot = _inventory.GetSlot(i);
            SlotView view = _slotViews[i];

            view.icon.enabled = !slot.IsEmpty;
            view.icon.sprite = slot.IsEmpty ? null : slot.item.icon;
            view.count.text = slot.IsEmpty ? "" : slot.count.ToString();
            view.selectionFrame.SetActive(i == _inventory.SelectedIndex);
        }
    }
}