using System;
using System.Collections;
using System.Collections.Generic;
using DataLayer;
using Storage;
using TMPro;
using UnityEngine;

public class DetailsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _detailsText;
    [SerializeField] private MainStorage _mainStorage;

    private EventBus _eventBus;
    void Start()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.DraggableTapped?.AddListener(OnDraggableTap);
    }

    private void OnDraggableTap(Draggable draggable)
    {
        if (draggable.TryGetComponent<InventoryItem>(out var item))
        {
            if (_mainStorage.ItemDescriptions.ContainsKey(item.UniqName)) {
                ItemDescription itemDescr = _mainStorage.ItemDescriptions[item.UniqName];
                _detailsText.text = itemDescr.description;
                _titleText.text = itemDescr.title;
            } else {
                _detailsText.text = "";
                _titleText.text = "";
            }
        }
    }

    private void OnDestroy()
    {
        _eventBus.DraggableTapped?.RemoveListener(OnDraggableTap);
    }
}
