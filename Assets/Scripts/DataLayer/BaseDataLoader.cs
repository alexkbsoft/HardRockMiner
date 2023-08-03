using System.Collections;
using System.Collections.Generic;
using DataLayer;
using Storage;
using UnityEditor;
using UnityEngine;

public class BaseDataLoader : MonoBehaviour
{
    [SerializeField] MainStorage _mainStorage;

    private EventBus _eventBus;

    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        
        LoadStorage();
        
        _eventBus.DataReady?.Invoke();
    }

    private void LoadStorage()
    {
        var dataManager = new DataManager();
        var storageDto = dataManager.LoadMainStorage();
        
        _mainStorage.resources.Clear();
        _mainStorage.InventoryItems.Clear();

        foreach (var resDto in storageDto.Resources)
        {
            _mainStorage.resources.Add(new MinerState.StoredResource()
            {
                name = resDto.name,
                count = resDto.count
            });
        }

        foreach (ResourceDto item in storageDto.Inventory)
        {
            _mainStorage.InventoryItems.Add(item);
        }

        Dictionary<string, string> tmpParts = new();

        foreach (var partDto in storageDto.MechParts)
        {
            tmpParts[partDto.name] = partDto.item;
        }

        _mainStorage.SetMechPartsDict(tmpParts);

        AddResourcesFromInventory(storageDto);
    }

    private void AddResourcesFromInventory(StorageDto storageDto) {
        foreach (var resDto in storageDto.Resources) {

        }
    }

    [ContextMenu("SaveStorage")]
    public void SaveStorage()
    {
        new DataManager().SaveMainStorage(_mainStorage);
    }
}
