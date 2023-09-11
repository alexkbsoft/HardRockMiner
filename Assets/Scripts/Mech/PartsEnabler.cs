using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;


public class PartsEnabler : MonoBehaviour
{
    [SerializeField] private List<GameObject> _replacebleParts;
    [SerializeField] private MainStorage _mainStorage;

    private GameObject _currentlySelected;
    private EventBus _eventBus;

    private void Awake()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.InventoryReordered?.AddListener(UpdateStructure);
        _eventBus.DataReady?.AddListener(UpdateStructure);
        _eventBus.UpdateMechStructure?.AddListener(UpdateStructure);
        _eventBus.DroppedInMech?.AddListener(PartDropped);
        _eventBus.DraggableTapped?.AddListener(OnDraggableTap);

    }

    private void OnDraggableTap(Draggable draggable)
    {
        var slot = draggable.Slot;
        var itemName = draggable.GetComponent<InventoryItem>().UniqName;

        if (slot != null &&
            !string.IsNullOrEmpty(slot.MechPartName))
        {
            PartDropped(draggable.Slot.MechPartName, itemName);
        }
    }

    public void UpdateStructure()
    {
        foreach (var partGO in _replacebleParts)
        {
            partGO.SetActive(_mainStorage.MechParts.ContainsValue(partGO.name));
        }
    }

    void OnDestroy()
    {
        _eventBus.InventoryReordered?.RemoveListener(UpdateStructure);
        _eventBus.DataReady?.RemoveListener(UpdateStructure);
        _eventBus.UpdateMechStructure?.RemoveListener(UpdateStructure);
        _eventBus.DroppedInMech?.RemoveListener(PartDropped);
        _eventBus.DraggableTapped?.RemoveListener(OnDraggableTap);

    }

    void PartDropped(string partName, string itemName)
    {
        var part = _replacebleParts.Find((GameObject onePart) => onePart.name == itemName);

        if (_currentlySelected != null)
        {
            Destroy(_currentlySelected.GetComponent<MyOutline>());
        }

        if (part != null)
        {
            part.AddComponent<MyOutline>();
            _currentlySelected = part;
        }

    }
}
