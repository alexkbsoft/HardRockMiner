using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using Utils;

public class Draggable : MonoBehaviour
{
    public bool IsDragging;
    public DragSlot Slot;
    public event Action<bool> OnDragChange;

    private Collider2D _collider;
    private DragController _dragController;
    private float _movementTime = 15;
    private Vector3? _movementDestination;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _dragController = FindObjectOfType<DragController>();

        if (Slot != null)
        {
            transform.position = Slot.transform.position;
        }
    }

    public void SetDragging(bool isDragging)
    {
        IsDragging = isDragging;
        OnDragChange?.Invoke(isDragging);
    }

    void FixedUpdate()
    {
        if (IsDragging)
        {
            return;
        }
        
        transform.position = Vector3.Lerp(
            transform.position,
            Slot.transform.position,
            _movementTime * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckValidDrop(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckValidDrop(other);
    }

    private void CheckValidDrop(Collider2D collider)
    {
        var area = GeometryUtils.IntersectArea(collider.bounds, _collider.bounds);

        if (collider.gameObject.TryGetComponent<DragSlot>(out var slot))
        {
            slot.CurrentIntersectArea = area;
            _dragController.CheckIfBetter(slot);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exit - " + other.gameObject.name);
        if (other.gameObject.TryGetComponent<DragSlot>(out var slot))
        {
            _dragController.ResetCurrentSlot(slot);
        }
    }
}
