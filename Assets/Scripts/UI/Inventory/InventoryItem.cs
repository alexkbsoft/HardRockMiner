using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public string UniqName;
    public string Description;
    public bool IsStackable = false;
    public int Count = 1;
    public bool IsCraftClone = false;
    public InventoryItem OriginalItem;

    [SerializeField] private GameObject BorderSprite;
    [SerializeField] private GameObject SelectedSprite;
    [SerializeField] private SpriteRenderer ItemSprite;
    [SerializeField] private TextMeshProUGUI _countText;

    private Draggable _draggable;

    void Start()
    {
        _draggable = GetComponent<Draggable>();
        _draggable.OnDragChange += OnDragChange;
        _draggable.OnSelectChange += OnSelectChange;
        _countText.gameObject.SetActive(IsStackable);

        UpdateText();
    }

    public void UpdateText()
    {
        if (IsStackable)
        {
            _countText.text = $"{Count}";
        }
    }

    public void CopyFrom(InventoryItem otherItem)
    {
        UniqName = otherItem.UniqName;
        Description = otherItem.Description;
        ItemSprite.sprite = Resources.Load<Sprite>(UniqName);
        IsStackable = otherItem.IsStackable;
    }

    public void GetFrom(InventoryItem otherItem, int count)
    {
        Count += count;
        otherItem.Count -= count;

        UpdateText();
        otherItem.UpdateText();
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

    public bool EmptySlot => IsStackable && Count == 0;
    public bool IsSingleItemAlreadyDropped => !IsStackable && Count < 0;
}
