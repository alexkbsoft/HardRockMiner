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
        _eventBus.SchemaReset?.AddListener(OnSchemaReset);
    }

    private void OnTabSelected(int tab) {
        if (tab != (int)InventoryTabs.Craft) {
            CleanAllSlots();
        }
    }

    private void CleanAllSlots() {
        foreach(DragSlot slot in Slots) {
            slot.Clean(true);
            slot.HideClue();
        }

        _eventBus.InventoryReordered?.Invoke();
        _eventBus.DroppedInCraft?.Invoke(null);
    }

    private void OnSchemaReset() {
        foreach(DragSlot slot in Slots) {
            slot.HideClue();
        }
    }

    void OnDestroy()
    {
        _eventBus.InventoryTabSelected?.RemoveListener(OnTabSelected);
        _eventBus.SchemaReset?.RemoveListener(OnSchemaReset);
    }
}
