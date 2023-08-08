using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DragSlot : MonoBehaviour
{
    public float CurrentIntersectArea;
    public Draggable LinkedDraggable;
    public InventoryItem LinkedItemCached;
    public string MechPartName;
    public bool IsCraftSlot;

    [SerializeField] private GameObject validImg;
    [SerializeField] private GameObject invalidImg;
    [SerializeField] private GameObject ItemPrefab;

    public void SetValidity(bool isValid)
    {
        if (isValid)
        {
            validImg.SetActive(true);
            invalidImg.SetActive(false);
        }
        else
        {
            invalidImg.SetActive(true);
            validImg.SetActive(false);
        }
    }

    public void Reset()
    {
        validImg.SetActive(false);
        invalidImg.SetActive(false);
        CurrentIntersectArea = 0;
    }

    public void Clean(bool returnResources = false)
    {
        if (LinkedDraggable != null)
        {
            var linkedItem = LinkedDraggable.GetComponent<InventoryItem>();

            if (linkedItem.OriginalItem != null && returnResources)
            {
                linkedItem.OriginalItem.RestoreOriginalCount();
            }
            
            Destroy(LinkedDraggable.gameObject);
        }

        LinkedDraggable = null;
        LinkedItemCached = null;
        Reset();
    }

    public void SetDraggable(Draggable draggable)
    {
        if (IsCraftSlot && !draggable.Slot.IsCraftSlot)
        {
            DublicateItem(draggable);
        }
        else
        {
            ExchangeWith(draggable);
        }
    }

    private void ExchangeWith(Draggable draggable)
    {
        if (LinkedDraggable != null && LinkedDraggable != draggable)
        {
            LinkedDraggable.Slot = draggable.Slot;
            draggable.Slot.LinkedDraggable = LinkedDraggable;
            draggable.Slot.LinkedItemCached = LinkedItemCached;
        }
        else if (LinkedDraggable == null)
        {
            draggable.Slot.LinkedDraggable = null;
            draggable.Slot.LinkedItemCached = null;
        }

        draggable.Slot = this;
        draggable.transform.parent = transform.parent;
        LinkedDraggable = draggable;
        LinkedItemCached = LinkedDraggable.GetComponent<InventoryItem>();
    }

    private void DublicateItem(Draggable draggable)
    {
        var otherItem = draggable.gameObject.GetComponent<InventoryItem>();

        if (otherItem.EmptySlot ||
            otherItem.IsSingleItemAlreadyDropped)
        {
            return;
        }

        LinkedItemCached = otherItem;

        GameObject itemInCrafting = FindItemInCraftOrCreate();

        var currentItem = itemInCrafting.GetComponent<InventoryItem>();
        var currentDraggable = itemInCrafting.GetComponent<Draggable>();

        currentDraggable.Slot = this;
        currentItem.CopyFrom(otherItem);
        currentItem.IsCraftClone = true;
        currentItem.OriginalItem = otherItem;
        LinkedDraggable = currentDraggable;
        currentItem.GetFrom(otherItem, 1);
    }

    private GameObject FindItemInCraftOrCreate()
    {
        return LinkedDraggable != null ?
            LinkedDraggable.gameObject : Instantiate(ItemPrefab, transform.parent);
    }
}