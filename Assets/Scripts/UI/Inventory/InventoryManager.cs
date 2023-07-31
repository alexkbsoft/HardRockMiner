using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Storage;
using UnityEngine;
using DG.Tweening;


public class InventoryManager : MonoBehaviour
{
    [SerializeField] private MainStorage MainStorage;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private DragSlot[] _slots;
    [SerializeField] private DragSlot[] _mechSlots;

    [SerializeField] private GameObject _inventoryContainer;
    [SerializeField] private GameObject _mechSlotsContainer;
    [SerializeField] private GameObject _craftSlotsContainer;
    [SerializeField] private GameObject _craftSchemaContainer;
    private EventBus _eventBus;
    private const float INITIAL_PANEL_POS = 0.6f;
    private const float OFFSCREEN_PANEL_POS = 8;
    private const float PANEL_ANIMATION_DURATION = 0.3f;

    private const float INITIAL_SCHEMA_PANEL_POS = 8;
    private const float OFFSCREEN_SCHEMA_PANEL_POS = 0;

    private void Awake()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.InventoryReordered?.AddListener(OnInventoryReordered);
        _eventBus.DataReady?.AddListener(RebuildInventory);
        _eventBus.InventoryTabSelected?.AddListener(TabSelected);
    }

    void Start()
    {
    }

    [ContextMenu("RebuildInventory")]
    private void RebuildInventory()
    {
        for (int oneItem = 0; oneItem < MainStorage.InventoryItems.Count; oneItem++)
        {
            var itemName = MainStorage.InventoryItems[oneItem];
            FillSlot(_slots[oneItem], itemName, _inventoryContainer.transform);
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

        var sprite = Resources.Load<Sprite>(itemName);
        var inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.UniqName = itemName;
        inventoryItem.SetSprite(sprite);

        if (MainStorage.StackableItems.Contains(itemName))
        {
            inventoryItem.IsStackable = true;
        }
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
        _eventBus.DataReady?.RemoveListener(RebuildInventory);
        _eventBus.InventoryTabSelected?.RemoveListener(TabSelected);
    }

    private void TabSelected(int index)
    {
        if (index == 1)
        {
            ShowCraft();
        }

        if (index == 2)
        {
            ShowMech();
        }
    }

    private void ShowCraft()
    {
        
        _mechSlotsContainer.transform.DOMoveY(OFFSCREEN_PANEL_POS, PANEL_ANIMATION_DURATION)
            .OnComplete(() =>
            {
                _craftSlotsContainer.transform.DOMoveY(INITIAL_PANEL_POS, PANEL_ANIMATION_DURATION);
                _craftSchemaContainer.transform.DOMoveX(INITIAL_SCHEMA_PANEL_POS, PANEL_ANIMATION_DURATION);

            });
    }

    private void ShowMech()
    {
        _craftSchemaContainer.transform.DOMoveX(OFFSCREEN_SCHEMA_PANEL_POS, PANEL_ANIMATION_DURATION);
        
        _craftSlotsContainer.transform.DOMoveY(OFFSCREEN_PANEL_POS, PANEL_ANIMATION_DURATION)
            .OnComplete(() => _mechSlotsContainer.transform.DOMoveY(INITIAL_PANEL_POS, PANEL_ANIMATION_DURATION));
    }
}
