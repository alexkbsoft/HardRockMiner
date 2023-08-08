using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using Unity.VisualScripting;
using UnityEngine;

public class ResourcesPanel : MonoBehaviour
{
    private List<GameObject> _resourcePanels = new();
    [SerializeField] private GameObject _resourcePrefab;
    [SerializeField] private MainStorage _storage;

    private EventBus _eventBus;

    private void Awake()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        
        _eventBus.DataReady?.AddListener(UpdatePanel);
        _eventBus.ResourcesUpdated?.AddListener(UpdatePanel);
    }

    private void UpdatePanel()
    {
        
        foreach (GameObject panel in _resourcePanels)
        {
            Destroy(panel);
        }
        _resourcePanels.Clear();

        int pos = 170;
        foreach (MinerState.StoredResource res in _storage.resources)
        {
            var newPanel = Instantiate(_resourcePrefab, transform);

            var resPanel = newPanel.GetComponent<ResourcePanel>();
            resPanel.UpdateUI(res.name, res.count, pos);
            pos += 200;
            _resourcePanels.Add(newPanel);
        }
    }

    private void OnDestroy()
    {
        _eventBus.DataReady?.RemoveListener(UpdatePanel);
        _eventBus.ResourcesUpdated?.RemoveListener(UpdatePanel);

    }
}
