using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Storage;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private MainStorage MainStorage;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private DragSlot[] _slots;
    [SerializeField] private DragSlot[] _mechSlots;

    private GameObject _inventoryContainer;
    private GameObject _mechSlotsContainer;
    private EventBus _eventBus;

    private void Awake()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.InventoryReordered?.AddListener(OnInventoryReordered);
        _eventBus.DataReady?.AddListener(RebuildInventory);
    }

    void Start()
    {
        _inventoryContainer = GameObject.Find("Inventory");
        _mechSlotsContainer = GameObject.Find("MechSlots");
    }

    [ContextMenu("RebuildInventory")]
    private void RebuildInventory()
    {
        for (int oneItem = 0; oneItem < MainStorage.InventoryItems.Count; oneItem++)
        {
            var itemName = MainStorage.InventoryItems[oneItem];
            FillSlot(_slots[oneItem], itemName, _inventoryContainer.transform);
            // if (string.IsNullOrEmpty(itemName))
            // {
            //     continue;
            // }
            //
            // var newItem = Instantiate(this.ItemPrefab);
            //
            // newItem.transform.parent = _inventoryContainer.transform;
            // var draggable = newItem.GetComponent<Draggable>();
            //
            // DragSlot slot = _slots[oneItem];
            // slot.LinkedDraggable = draggable;
            // draggable.Slot = slot;
            //
            // var sprite  = Resources.Load<Sprite>(itemName);
            // var inventoryItem = newItem.GetComponent<InventoryItem>();
            // inventoryItem.UniqName = itemName;
            // inventoryItem.SetSprite(sprite);
        }

        foreach (var partPair in MainStorage.MechParts)
        {
            var slot = _mechSlots.First(item => item.MechPartName == partPair.Key);
            FillSlot(slot, partPair.Value, _mechSlotsContainer.transform);
        }
    }

    private void FillSlot(DragSlot slot, string itemName, Transform container)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return;
        }
            
        var newItem = Instantiate(this.ItemPrefab, container);
        var draggable = newItem.GetComponent<Draggable>();
        
        slot.LinkedDraggable = draggable;
        draggable.Slot = slot;
            
        var sprite  = Resources.Load<Sprite>(itemName);
        var inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.UniqName = itemName;
        inventoryItem.SetSprite(sprite);
    }

    private void OnInventoryReordered()
    {
        var tmpList = new List<string>();
        
        for (int i = 0; i < _slots.Length; i++)
        {
            var draggable = _slots[i].LinkedDraggable;

            if (draggable != null)
            {
                string linkedItemId = _slots[i].LinkedDraggable.GetComponent<InventoryItem>().UniqName;
                tmpList.Add(linkedItemId);
            }
            else
            {
                tmpList.Add(null);
            }
        }

        MainStorage.InventoryItems = tmpList;
        
        Dictionary<string, string> tmpParts = new();


        foreach (var mechSlot in _mechSlots)
        {
            if (!string.IsNullOrEmpty(mechSlot.MechPartName) 
                && mechSlot.LinkedDraggable != null)
            {
                var uniqItemName = mechSlot.LinkedDraggable.GetComponent<InventoryItem>().UniqName;
                tmpParts[mechSlot.MechPartName] = uniqItemName;
            }
        }
        
        MainStorage.SetMechPartsDict(tmpParts);

        var dataManager = new DataManager();
        dataManager.SaveMainStorage(MainStorage);
    }

    private void OnDestroy()
    {
        _eventBus.InventoryReordered?.RemoveListener(OnInventoryReordered);
    }
}
