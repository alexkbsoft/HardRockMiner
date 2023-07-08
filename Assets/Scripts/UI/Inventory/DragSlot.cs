using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DragSlot : MonoBehaviour
{
    public float CurrentIntersectArea;
    public Draggable LinkedDraggable;
    
    
    [SerializeField] private GameObject validImg;
    [SerializeField] private GameObject invalidImg;

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
    }

    public void SetDraggable(Draggable draggable)
    {
        if (LinkedDraggable != null && LinkedDraggable != draggable)
        {
            LinkedDraggable.Slot = draggable.Slot;
            draggable.Slot.LinkedDraggable = LinkedDraggable;
        } else if (LinkedDraggable == null)
        {
            draggable.Slot.LinkedDraggable = null;
        }
        LinkedDraggable = draggable;
        draggable.Slot = this;
    }
}
