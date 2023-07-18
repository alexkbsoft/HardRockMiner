using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer;
using Storage;
using UnityEngine;

public class DataManager
{
    public void SaveCurrentAsteroid(string asteroidName)
    {
        var path = Path.Combine(Application.persistentDataPath, $"{asteroidName}_asteroid.json");
        
        var gameDto = CreateDTO();
        var jsonString = JsonUtility.ToJson(gameDto, true);
        
        File.WriteAllText(path, jsonString);
    }

    public void SaveMainStorage(MainStorage storage)
    {
        var path = Path.Combine(Application.persistentDataPath, $"main_storage.json");

        var storageDto = CreateStorageDto(storage);
        var jsonString = JsonUtility.ToJson(storageDto, true);
        
        File.WriteAllText(path, jsonString);
    }

    public StorageDto LoadMainStorage()
    {
        var path = Path.Combine(Application.persistentDataPath, $"main_storage.json");

        var jsonString = File.ReadAllText(path);
        return JsonUtility.FromJson<StorageDto>(jsonString);
    }

    public GameDto LoadAsteroid(string asteroidName)
    {
        var path = Path.Combine(Application.persistentDataPath, $"{asteroidName}_asteroid.json");

        var jsonString = File.ReadAllText(path);
        return JsonUtility.FromJson<GameDto>(jsonString);
    }

    public bool IsAsteroidExists(string asteroidName)
    {
        var path = Path.Combine(Application.persistentDataPath, $"{asteroidName}_asteroid.json");

        return File.Exists(path);
    }

    public string[] AsteroidNames()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = dir.GetFiles("*_asteroid.json");

        return files.Select(oneFile => oneFile.Name.Replace("_asteroid.json", "")).ToArray();
    }

    private GameDto CreateDTO()
    {
        var blocks = GameObject.FindObjectsOfType<ResourceBlock>();

        List<BlockDto> blockDtos = new List<BlockDto>();

        foreach (var block in blocks)
        {
            Damagable dmg = block.gameObject.GetComponent<Damagable>();
            var position = block.transform.position;
            
            BlockDto dto = new()
            {
                Life = dmg.CurrentLife,
                Type = block.BlockType,
                X = position.x,
                Z = position.z
            };
            
            blockDtos.Add(dto);
        }

        return new GameDto()
        {
            Blocks = blockDtos
        };
    }

    private StorageDto CreateStorageDto(MainStorage storage)
    {
        List<ResourceDto> resources = new List<ResourceDto>();
        List<MechPartDto> mechParts = new();

        foreach (MinerState.StoredResource res in storage.resources)
        {
            resources.Add(new ResourceDto()
            {
                name = res.name,
                count = res.count
            });
        }

        foreach (var onePart in storage.MechParts)
        {
            mechParts.Add(new MechPartDto()
            {
                name = onePart.Key,
                item = onePart.Value
            });
        }

        return new StorageDto()
        {
            Resources = resources,
            Inventory = storage.InventoryItems,
            MechParts = mechParts
        };
    }

}
