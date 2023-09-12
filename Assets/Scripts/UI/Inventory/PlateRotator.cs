using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[Serializable]
public class PlatePosition
{
    public Vector3 position;
    public Quaternion rotation;
}

public class PlateRotator : MonoBehaviour
{
    [SerializeField] private Quaternion _targetRotation = Quaternion.identity;
    [SerializeField] private Vector3 _targetPosition = Vector3.zero;
    [SerializeField] private float Speed = 2.0f;

    [SerializedDictionary("Slot name", "Position")] public SerializedDictionary<string, PlatePosition> PositionsDictionary;

    private EventBus _eventBus;

    void Start()
    {
        _eventBus = EventBus.Instance;

        _eventBus.DroppedInMech?.AddListener(OnDropInMech);
        _eventBus.DraggableTapped?.AddListener(OnDraggableTap);
    }

    private void OnDropInMech(string part, string itemName)
    {
        FocusMechPart(part);
    }

    private void OnDraggableTap(Draggable draggable)
    {
        if (draggable.Slot != null &&
            !string.IsNullOrEmpty(draggable.Slot.MechPartName))
        {
            FocusMechPart(draggable.Slot.MechPartName);
        }
    }

    private void FocusMechPart(string part)
    {

        PlatePosition pos = PositionsDictionary[part];

        if (pos != null)
        {
            _targetPosition = pos.position;
            _targetRotation = pos.rotation;
        }
    }

    void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Speed * Time.deltaTime);
        transform.localPosition = Vector3.Slerp(transform.localPosition, _targetPosition, Speed * Time.deltaTime);
    }

    void OnDestroy()
    {
        _eventBus.DroppedInMech?.RemoveListener(OnDropInMech);
        _eventBus.DraggableTapped?.RemoveListener(OnDraggableTap);
    }
}
