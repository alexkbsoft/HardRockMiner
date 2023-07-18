using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public string UniqName;
    public string Description;

    [SerializeField] private GameObject BorderSprite;
    [SerializeField] private GameObject SelectedSprite;
    [SerializeField] private SpriteRenderer ItemSprite;
    
    private Draggable _draggable;
    
    void Start()
    {
        _draggable = GetComponent<Draggable>();
        _draggable.OnDragChange += OnDragChange;
        _draggable.OnSelectChange += OnSelectChange;
    }

    public void SetSprite(Sprite sprite)
    {
        ItemSprite.sprite = sprite;
    }

    private void OnDragChange(bool isDragging)
    {
        BorderSprite.SetActive(isDragging);
    }
    
    private void OnSelectChange(bool isDragging)
    {
        SelectedSprite.SetActive(isDragging);
    }

    private void OnDestroy()
    {
        _draggable.OnDragChange -= OnDragChange;
        _draggable.OnSelectChange -= OnSelectChange;
    }
}
