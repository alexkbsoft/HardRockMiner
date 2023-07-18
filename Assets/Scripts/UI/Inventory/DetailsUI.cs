using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _detailsText;

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
            _detailsText.text = item.Description;
        }
    }

    private void OnDestroy()
    {
        _eventBus.DraggableTapped?.RemoveListener(OnDraggableTap);
    }
}
