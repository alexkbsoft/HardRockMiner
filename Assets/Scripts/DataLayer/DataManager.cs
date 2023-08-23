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

        Debug.Log("path" + path);

        var jsonString = File.ReadAllText(path);
        return JsonUtility.FromJson<StorageDto>(jsonString);
    }

    public GameDto LoadAsteroid(string asteroidName)
    {
        var path = Path.Combine(Application.persistentDataPath, $"{asteroidName}_asteroid.json");

        var jsonString = File.ReadAllText(path);
        return JsonUtility.FromJson<GameDto>(jsonString);
    }

    public void DeleteAsteroid(string asteroidName)
    {
        var path = Path.Combine(Application.persistentDataPath, $"{asteroidName}_asteroid.json");

        File.Delete(path);
    }

    public bool IsAsteroidExists(string asteroidName)
    {
        var path = Path.Combine(Application.persistentDataPath, $"{asteroidName}_asteroid.json");

        return File.Exists(path);
    }

    public bool IsMainStorageExists()
    {
        var path = Path.Combine(Application.persistentDataPath, $"main_storage.json");

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
        var walls = GameObject.FindObjectsOfType<WallTag>();
        var floors = GameObject.FindObjectsOfType<FloorTag>();
        var decors = GameObject.FindObjectsOfType<CaveDecoration>();
        var spawners = GameObject.FindObjectsOfType<EnemySpawner>();
        var pillars = GameObject.FindObjectsOfType<PillarTag>();

        List<BlockDto> blockDtos = new List<BlockDto>();
        List<SegmentDto> wallDtos = new List<SegmentDto>();
        List<SegmentDto> floorsDtos = new List<SegmentDto>();
        List<SegmentDto> decorationDtos = new List<SegmentDto>();
        List<SegmentDto> spawnersDtos = new List<SegmentDto>();
        List<SegmentDto> pillarDtos = new List<SegmentDto>();

        foreach (var block in blocks)
        {
            Damagable dmg = block.gameObject.GetComponent<Damagable>();
            var position = block.transform.position;

            BlockDto dto = new()
            {
                Life = dmg.CurrentLife,
                Type = block.BlockType,
                X = position.x,
                Y = position.y,
                Z = position.z
            };

            blockDtos.Add(dto);
        }

        foreach (var wall in walls)
        {
            var pos = wall.transform.position;

            SegmentDto seg = new()
            {
                Type = wall.Name,
                YRotation = wall.transform.rotation.eulerAngles.y,
                X = pos.x,
                Y = pos.y,
                Z = pos.z
            };

            wallDtos.Add(seg);
        }

        foreach (var fl in floors)
        {
            var pos = fl.transform.position;

            SegmentDto seg = new()
            {
                Type = fl.Name,
                YRotation = fl.transform.rotation.eulerAngles.y,
                X = pos.x,
                Y = pos.y,
                Z = pos.z
            };

            floorsDtos.Add(seg);
        }

        foreach (var oneDecor in decors)
        {
            var pos = oneDecor.transform.position;

            SegmentDto seg = new()
            {
                Type = oneDecor.UniqName,
                YRotation = oneDecor.transform.rotation.eulerAngles.y,
                X = pos.x,
                Y = pos.y,
                Z = pos.z
            };

            decorationDtos.Add(seg);
        }

        foreach (var oneSpawner in spawners)
        {
            var pos = oneSpawner.transform.position;

            SegmentDto seg = new()
            {
                Type = oneSpawner.UniqName,
                YRotation = oneSpawner.transform.rotation.eulerAngles.y,
                X = pos.x,
                Y = pos.y,
                Z = pos.z
            };

            spawnersDtos.Add(seg);
        }

        foreach (var onePillar in pillars)
        {
            var pos = onePillar.transform.position;

            SegmentDto seg = new()
            {
                Type = onePillar.UniqName,
                YRotation = onePillar.transform.rotation.eulerAngles.y,
                X = pos.x,
                Y = pos.y,
                Z = pos.z
            };

            pillarDtos.Add(seg);
        }

        return new GameDto()
        {
            Blocks = blockDtos,
            Walls = wallDtos,
            Floors = floorsDtos,
            Decorations = decorationDtos,
            Spawners = spawnersDtos,
            Pillars = pillarDtos
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
