using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class DragController : MonoBehaviour
{
    public Draggable LastDragged => _lastDragged;
    public DragSlot CurrentSlot => _currentSlot;

    private bool _isDragActive = false;
    private Vector2 _screenPosition;
    private Vector2 _worldPosition;
    private Draggable _lastDragged;
    private DragSlot _currentSlot;
    private Camera _camera;
    private Vector2 _clickPos;

    private EventBus _eventBus;

    private void Start()
    {
        _camera = GameObject.Find("InventoryCamera").GetComponent<Camera>();
        _eventBus = GameObject.FindObjectOfType<EventBus>();
    }

    void Update()
    {
        if (_isDragActive)
        {
            if (Input.GetMouseButtonUp(0) ||
                (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                Drop();

                return;
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            _screenPosition = new Vector2(mousePos.x, mousePos.y);
        }
        else if (Input.touchCount > 0)
        {
            _screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            return;
        }

        _worldPosition = _camera.ScreenToWorldPoint(_screenPosition);

        if (_isDragActive)
        {
            if (!_lastDragged.IsDragging && _screenPosition != _clickPos)
            {
                StartActualDragging(true);
            }

            if (_lastDragged.IsDragging)
            {
                Drag();
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(_worldPosition,
                Vector2.zero,
                20,
                1 << Layer.NotDragging);

            if (hit.collider != null)
            {
                Draggable draggable = hit.transform.gameObject.GetComponent<Draggable>();

                if (draggable != null && draggable.IsDragEnabled)
                {
                    if (_lastDragged != null && draggable != _lastDragged)
                    {
                        _lastDragged.SetSelected(false);
                    }

                    _clickPos = _screenPosition;
                    _lastDragged = draggable;
                    SelectDraggable(draggable);
                    InitDrag();
                }
            }
        }
    }

    private void SelectDraggable(Draggable draggable)
    {
        draggable.SetSelected(true);
    }

    void InitDrag()
    {
        UpdateDragStatus(true);
    }

    public void CheckIfBetter(DragSlot slot)
    {
        if (slot == _currentSlot)
        {
            return;
        }

        if (_currentSlot == null ||
            _currentSlot.CurrentIntersectArea < slot.CurrentIntersectArea)
        {
            if (_currentSlot != null)
            {
                _currentSlot.Reset();
            };

            bool isValid = IsValidTargertSlot(slot);
            slot.SetValidity(isValid);

            _currentSlot = slot;
        }
    }

    public void ResetCurrentSlot(DragSlot slot)
    {
        if (_currentSlot != null && slot == _currentSlot)
        {
            _currentSlot.Reset();
            _currentSlot = null;
        }
    }

    void Drag()
    {
        _lastDragged.transform.position = new Vector2(_worldPosition.x, _worldPosition.y);
    }

    void Drop()
    {
        UpdateDragStatus(false);
        StartActualDragging(false);

        if (_currentSlot != null)
        {
            bool isChangeInCraft = _currentSlot.IsCraftSlot;

            if (IsValidTargertSlot(_currentSlot)) {
                _currentSlot.SetDraggable(_lastDragged);

                if (!string.IsNullOrEmpty(_currentSlot.MechPartName)) {
                    InventoryItem item = _lastDragged.gameObject.GetComponent<InventoryItem>();

                    _eventBus.DroppedInMech?.Invoke(_currentSlot.MechPartName, item.UniqName);
                }
            } else {
                DeleteClone();
            }

            ResetCurrentSlot(_currentSlot);
        }
        else
        {
            DeleteClone();
        }

        _eventBus.DroppedInCraft?.Invoke();
        _eventBus.InventoryReordered?.Invoke();
    }

    private void DeleteClone()
    {
        InventoryItem item = _lastDragged.gameObject.GetComponent<InventoryItem>();

        if (item.IsCraftClone)
        {
            item.OriginalItem.GetFrom(item, item.Count);
            _lastDragged.Slot.LinkedDraggable = null;
            _lastDragged.Slot = null;
            Destroy(_lastDragged.gameObject);
            _lastDragged = null;
        }
    }


    void UpdateDragStatus(bool isDragging)
    {
        _isDragActive = isDragging;
    }

    void StartActualDragging(bool dragging)
    {
        _lastDragged.gameObject.layer = dragging ? Layer.Dragging : Layer.NotDragging;
        _lastDragged.SetDragging(dragging);
    }

    private bool IsValidTargertSlot(DragSlot targetSlot)
    {
        if (_lastDragged == null)
        {
            return false;
        }

        InventoryItem item = _lastDragged.gameObject.GetComponent<InventoryItem>();
        bool result = true;

        if (item.IsCraftClone && !targetSlot.IsCraftSlot)
        {
            result = false;
        }

        return result;
    }
}
