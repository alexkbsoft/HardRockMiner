using System.Collections;
using System.Collections.Generic;
using DataLayer;
using Storage;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;

    [SerializeField] MinerState _minerState;
    [SerializeField] MainStorage _mainStorage;
    [SerializeField] private bool _loadOnStart = true;

    private EventBus _eventBus;
    private bool _haveActiveSpawner = false;

    private static GameManager _instance;
    private CaveBuilder _caveBuilder;

    void Start()
    {
        _instance = this;

        _minerState.Reset();

        _eventBus = FindObjectOfType<EventBus>();
        _caveBuilder = FindObjectOfType<CaveBuilder>();

        _eventBus.AlarmChanged?.AddListener(OnAlarmChanged);
        _eventBus.ActivateSpawner?.AddListener(SpawnerActivated);
        _eventBus.MapGenerationDone?.AddListener(ScanMap);

        LoadStorage();

        bool isExists = false;

        if (_loadOnStart)
        {
            isExists = LoadLevel();
        }

        if (isExists)
        {
            ScanMap();
        }
        else
        {
            GenerateLevel();
        }

        _eventBus.DataReady?.Invoke();
    }

    public void ScanMap()
    {
        GameObject caveGO = GameObject.Find("Cave");

        var finishPrefab = Resources.Load<GameObject>("FinishPoint");


        var (xStart, yStart) = _caveBuilder.GetPlayerPosition();
        MechController.Instance.SetInitialPlace(xStart, yStart);

        Instantiate(
            finishPrefab,
            new Vector3(xStart, 0, yStart) + Vector3.up * 2.73f - Vector3.forward * 5.0f,
            Quaternion.identity,
            caveGO.transform);

        StartCoroutine(RescanNavigation());
    }

    public void GenerateLevel()
    {
        _caveBuilder.Generate();
    }

    [ContextMenu("Save asteroid")]
    public void SaveAsteroid()
    {
        var dataManager = new DataManager();
        dataManager.SaveCurrentAsteroid(_minerState.AsteroidName);

        SceneManager.LoadScene("ResultScreen");
    }

    private void OnAlarmChanged(float delta)
    {
        if (!_haveActiveSpawner)
        {
            return;
        }

        var currntAlarm = _minerState.CurrentAlarm;

        if (currntAlarm < 1)
        {
            _minerState.AddAlarm(delta);

            if (_minerState.CurrentAlarm >= 1)
            {
                _eventBus.AlarmInvoked?.Invoke(true);
            }
        }

        if (currntAlarm > 1)
        {
            _minerState.CurrentAlarm = 1;
        }
    }

    private void SpawnerActivated(bool activated)
    {
        _haveActiveSpawner = true;
    }



    void OnDestroy()
    {
        _eventBus.AlarmChanged?.RemoveListener(OnAlarmChanged);
        _eventBus.ActivateSpawner?.RemoveListener(SpawnerActivated);
        _eventBus.MapGenerationDone?.RemoveListener(ScanMap);



    }

    private bool LoadLevel()
    {
        var dataManager = new DataManager();
        if (!dataManager.IsAsteroidExists(_minerState.AsteroidName))
        {
            _mainStorage.SetDefaults();

            return false;
        }

        var levelData = dataManager.LoadAsteroid(_minerState.AsteroidName);

        var walls = GameObject.Find("Cave");

        var blockPrefabs = new Dictionary<string, GameObject>()
        {
            ["DirtBlock"] = Resources.Load("DirtBlock") as GameObject,
            ["BrickBlock"] = Resources.Load("BrickBlock") as GameObject,
            ["RockBlock"] = Resources.Load("RockBlock") as GameObject,
        };

        var wallPrefabs = new Dictionary<string, GameObject>()
        {
            ["Wall1"] = Resources.Load("Wall1") as GameObject,
            ["Wall2"] = Resources.Load("Wall2") as GameObject,
            ["Wall3"] = Resources.Load("Wall3") as GameObject,
        };

        var floorPrefabs = new Dictionary<string, GameObject>()
        {
            ["Floor"] = Resources.Load("Floor") as GameObject,
        };

        foreach (BlockDto blockDto in levelData.Blocks)
        {
            var newBlock = Instantiate(blockPrefabs[blockDto.Type],
                new Vector3(blockDto.X, blockDto.Y, blockDto.Z),
                Quaternion.identity);
            var resourceBlock = newBlock.GetComponent<ResourceBlock>();

            var damagable = newBlock.GetComponent<Damagable>();
            damagable.CurrentLife = blockDto.Life;
            newBlock.transform.parent = walls.transform;

            resourceBlock.ChooseAppearance(blockDto.Life);
        }

        foreach (SegmentDto wallDto in levelData.Walls)
        {
            var newWall = Instantiate(wallPrefabs[wallDto.Type],
                new Vector3(wallDto.X, wallDto.Y, wallDto.Z),
                Quaternion.Euler(0, wallDto.YRotation, 0));

            newWall.transform.parent = walls.transform;
        };

        foreach (SegmentDto floorDto in levelData.Floors)
        {
            var newFloor = Instantiate(floorPrefabs[floorDto.Type],
                new Vector3(floorDto.X, floorDto.Y, floorDto.Z),
                Quaternion.Euler(0, floorDto.YRotation, 0));

            newFloor.transform.parent = walls.transform;
        };

        return true;
    }

    private void LoadStorage()
    {
        var dataManager = new DataManager();

        if (!dataManager.IsMainStorageExists())
        {
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

        _mainStorage.SetMechPartsDict(tmpParts);
    }

    private IEnumerator RescanNavigation()
    {
        yield return new WaitForEndOfFrame();

        _eventBus.ScanNavigationGrid?.Invoke();

        _eventBus.MapReady?.Invoke();
    }
}
