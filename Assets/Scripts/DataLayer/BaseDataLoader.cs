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
        _eventBus = EventBus.Instance;

        LoadStorage();

        _eventBus.DataReady?.Invoke();
    }

    private void LoadStorage()
    {
        var dataManager = new DataManager();

        var items = dataManager.LoadItemDescriptions();

        _mainStorage.ItemDescriptions = new();
        _mainStorage.Recipies = new();
        _mainStorage.Schemas = new();

        foreach (ItemDescriptionDto descr in items.Descriptions)
        {
            _mainStorage.ItemDescriptions[descr.id] = new ItemDescription
            {
                title = descr.title,
                description = descr.description
            };
        }

        if (!dataManager.IsMainStorageExists())
        {
            _mainStorage.SetDefaults();

            return;
        }

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

        var recipes = dataManager.LoadRecipes();

        foreach (var oneRecipe in recipes.Recipes)
        {
            _mainStorage.Recipies.Add(oneRecipe);
        }

        var schemas = dataManager.LoadSchemas();

        foreach (var oneSchema in schemas.Schemas)
        {
            _mainStorage.Schemas[oneSchema.Id] = oneSchema.Hints;
        }

        _mainStorage.SetMechPartsDict(tmpParts);
    }

    [ContextMenu("SaveStorage")]
    public void SaveStorage()
    {
        new DataManager().SaveMainStorage(_mainStorage);
    }
}
