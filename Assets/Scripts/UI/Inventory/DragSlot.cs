using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSlot : MonoBehaviour
{
    public float CurrentIntersectArea;
    public Draggable CurrentDragging;
    
    
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
        CurrentDragging = draggable;
        draggable.Slot = this;
    }
}
