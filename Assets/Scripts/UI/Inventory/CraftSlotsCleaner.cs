using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSlotsCleaner : MonoBehaviour
{
    public DragSlot[] Slots;

    private EventBus _eventBus;


    void Start()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.InventoryTabSelected?.AddListener(OnTabSelected);
    }

    private void OnTabSelected(int tab) {
        if (tab != (int)InventoryTabs.Craft) {
            CleanAllSlots();
        }
    }

    private void CleanAllSlots() {
        foreach(DragSlot slot in Slots) {
            slot.Clean();
        }

        _eventBus.InventoryReordered?.Invoke();
    }

    void OnDestroy()
    {
        _eventBus.InventoryTabSelected?.AddListener(OnTabSelected);
    }
}
