using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Storage;
using UnityEngine;
using DG.Tweening;
using DataLayer;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private MainStorage MainStorage;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private DragSlot[] _slots;
    [SerializeField] private DragSlot[] _mechSlots;
    [SerializeField] private DragSlot[] _craftSlots;
    [SerializeField] private DragSlot _craftResult;

    [SerializeField] private GameObject _inventoryContainer;
    [SerializeField] private GameObject _mechSlotsContainer;
    [SerializeField] private GameObject _craftSlotsContainer;
    [SerializeField] private GameObject _craftSchemaContainer;
    [SerializeField] private GameObject _craftBtn;


    private EventBus _eventBus;

    private const float INITIAL_PANEL_POS = 0.6f;
    private const float OFFSCREEN_PANEL_POS = 10;
    private const float PANEL_ANIMATION_DURATION = 0.3f;

    private const float INITIAL_SCHEMA_PANEL_POS = 8;
    private const float OFFSCREEN_SCHEMA_PANEL_POS = 0;
    private CraftHelper _currentCraft;

    private void Awake()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.InventoryReordered?.AddListener(OnInventoryReordered);
        _eventBus.DataReady?.AddListener(RebuildInventory);
        _eventBus.InventoryTabSelected?.AddListener(TabSelected);
        _eventBus.DroppedInCraft?.AddListener(CraftReordered);
    }

    public void OnCraftClick()
    {
        if (_currentCraft == null)
        {
            return;
        }

        List<string> resources = _currentCraft.Recipie;

        MainStorage.SubtractResources(_currentCraft.Recipie);
        AddItem(_currentCraft.ItemName,
            MainStorage.StackableItems.Contains(_currentCraft.ItemName),
            1, "add");

        var dataManager = new DataManager();
        dataManager.SaveMainStorage(MainStorage);

        CleanAllSlots();
        RebuildInventory();

        _eventBus.UpdateMechStructure?.Invoke();
        _eventBus.ResourcesUpdated?.Invoke();
    }

    private void CleanAllSlots()
    {
        foreach (var oneSlot in _slots) oneSlot.Clean();
        foreach (var oneSlot in _craftSlots) oneSlot.Clean();
        foreach (var oneSlot in _mechSlots) oneSlot.Clean();
    }

    private void CraftReordered()
    {
        _craftResult.Clean();
        string foundRecipieItem = null;
        _currentCraft = null;

        foreach (Storage.RecipieItem oneRecipie in MainStorage.Recipies)
        {
            var craftHelper = new CraftHelper(oneRecipie.Resources, _craftSlots, oneRecipie.Id);

            if (craftHelper.IsMatch())
            {
                _currentCraft = craftHelper;

                break;
            }
        }

        if (_currentCraft != null)
        {
            var (inventoryItem, draggable) = FillSlot(_craftResult,
                _currentCraft.ItemName,
                _craftSchemaContainer.transform);

            draggable.IsDragEnabled = false;
            inventoryItem.transform.localScale = Vector3.one;
            inventoryItem.IsStackable = false;
        }

        _craftBtn.SetActive(_currentCraft != null);

    }


    private void RebuildInventory()
    {
        FillInventoryWithResources();

        for (int oneItem = 0; oneItem < MainStorage.InventoryItems.Count; oneItem++)
        {
            var item = MainStorage.InventoryItems[oneItem];
            FillSlot(_slots[oneItem], item.name, _inventoryContainer.transform, item.count);
        }

        foreach (var partPair in MainStorage.MechParts)
        {
            var slot = _mechSlots.First(item => item.MechPartName == partPair.Key);
            FillSlot(slot, partPair.Value, _mechSlotsContainer.transform);
        }
    }

    private void FillInventoryWithResources()
    {
        foreach (MinerState.StoredResource resDto in MainStorage.resources)
        {
            AddItem(resDto.name, true, resDto.count);
        }
    }

    private void AddItem(string name, bool stackable = false, int count = 1, string op = "set")
    {
        // Debug.Log("AddItem " + name + " " + stackable + " " + count + " " + op);

        if (stackable)
        {
            var inventoryItem = FindInInventory(name);

            if (inventoryItem == null)
            {
                CreateInFreeSlot(name, count);
            }
            else
            {
                if (op == "set") {
                    inventoryItem.count = count;
                } else if (op == "add") {
                    inventoryItem.count += count;
                }
            }
        }
        else
        {
            CreateInFreeSlot(name, count);
        }
    }

    private void CreateInFreeSlot(string name, int count)
    {
        var inventorySlot = MainStorage.FindFreeInventorySlot();

        if (inventorySlot != null)
        {
            inventorySlot.name = name;
            inventorySlot.count = count;
        }
    }

    private ResourceDto FindInInventory(string name)
    {
        return MainStorage.InventoryItems.Find((res) => res.name == name);
    }

    private (InventoryItem, Draggable) FillSlot(DragSlot slot, string itemName, Transform container, int count = 0)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return (null, null);
        }

        var newItem = Instantiate(this.ItemPrefab, container);
        var draggable = newItem.GetComponent<Draggable>();

        slot.LinkedDraggable = draggable;
        draggable.Slot = slot;

        var sprite = Resources.Load<Sprite>(itemName);
        var inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.Configure(itemName, "", sprite, count);

        if (MainStorage.StackableItems.Contains(itemName))
        {
            inventoryItem.IsStackable = true;
        }

        return (inventoryItem, draggable);
    }

    private void OnInventoryReordered()
    {
        var tmpList = new List<ResourceDto>();

        for (int i = 0; i < _slots.Length; i++)
        {
            var draggable = _slots[i].LinkedDraggable;

            if (draggable != null)
            {
                var item = _slots[i].LinkedDraggable.GetComponent<InventoryItem>();


                tmpList.Add(new ResourceDto
                {
                    name = item.UniqName,
                    count = item.Count
                });
            }
            else
            {
                tmpList.Add(new ResourceDto
                {
                    name = null,
                    count = 0
                });
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
        _eventBus.DroppedInCraft?.RemoveListener(CraftReordered);
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
