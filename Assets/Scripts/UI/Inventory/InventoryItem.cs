using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private GameObject BorderSprite;
    private Draggable _draggable;
    
    void Start()
    {
        _draggable = GetComponent<Draggable>();
        _draggable.OnDragChange += OnDragChange;
    }

    private void OnDragChange(bool isDragging)
    {
        BorderSprite.SetActive(isDragging);
    }

    private void OnDestroy()
    {
        _draggable.OnDragChange -= OnDragChange;
    }
}
