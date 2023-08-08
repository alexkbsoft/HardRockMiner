using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;

public class PartsEnabler : MonoBehaviour
{
    [SerializeField] private List<GameObject> _replacebleParts;
    [SerializeField] private MainStorage _mainStorage;

    private EventBus _eventBus;

    private void Awake()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.InventoryReordered?.AddListener(UpdateStructure);
        _eventBus.DataReady?.AddListener(UpdateStructure);
        _eventBus.UpdateMechStructure?.AddListener(UpdateStructure);
    }

    public void UpdateStructure()
    {
        foreach (var partGO in _replacebleParts)
        {
            partGO.SetActive(_mainStorage.MechParts.ContainsValue(partGO.name));
        }
    }

    void OnDestroy()
    {
        _eventBus.InventoryReordered?.RemoveListener(UpdateStructure);
        _eventBus.DataReady?.RemoveListener(UpdateStructure);
        _eventBus.UpdateMechStructure?.RemoveListener(UpdateStructure);
    }
}
