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
    private int _storedCount;

    void Start()
    {
        _draggable = GetComponent<Draggable>();
        _draggable.OnDragChange += OnDragChange;
        _draggable.OnSelectChange += OnSelectChange;
        _countText.gameObject.SetActive(IsStackable);

        UpdateText();
    }

    public void RemoveSelf() {
        _draggable.Slot.Clean();
    }

    public void UpdateText()
    {
        if (IsStackable && !IsCraftClone)
        {
            _countText.text = $"{Count}";
        }

        if (IsCraftClone) {
            _countText.gameObject.SetActive(false);
        }
    }

    public void CopyFrom(InventoryItem otherItem)
    {
        UniqName = otherItem.UniqName;
        Description = otherItem.Description;
        ItemSprite.sprite = otherItem.GetSprite();
        IsStackable = otherItem.IsStackable;
        _storedCount = otherItem.StoredCount;
    }

    public int StoredCount {
        get => _storedCount;
    }

    public void RestoreOriginalCount () {
        Count = _storedCount;
        UpdateText();
    }

    public void SetStoredCount(int count) {
        _storedCount = count;
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

    public Sprite GetSprite()
    {
        return ItemSprite.sprite;
    }

    public void Configure(string name,
        string description,
        Sprite sprite,
        int count = 0,
        bool isStackable = false)
    {
        UniqName = name;
        Description = description;
        ItemSprite.sprite = sprite;
        Count = count;
        IsStackable = isStackable;
        _storedCount = count;
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
