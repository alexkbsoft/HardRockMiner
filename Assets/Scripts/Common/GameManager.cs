using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataLayer;
using Lean.Pool;
using Storage;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;
    public MechController _playerMech;

    [SerializeField] MinerState _minerState;
    [SerializeField] MainStorage _mainStorage;
    [SerializeField] private bool _debugMap = false;

    private EventBus _eventBus;
    private bool _haveActiveSpawner = false;

    private static GameManager _instance;
    private CaveBuilder _caveBuilder;
    private Dictionary<string, GameObject> ResPrefabs = new();

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

        if (_debugMap)
        {
            _eventBus.DataReady?.Invoke();

            return;
        }

        bool isExists = LoadLevel();

        if (isExists)
        {
            ScanMap();
        }
        else
        {
            GenerateLevel();
        }

        PrepareResourcesPool();

        _eventBus.DataReady?.Invoke();
    }

    public void Clean()
    {
        GameObject caveGO = GameObject.Find("Cave");

        foreach (Transform t in caveGO.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void ScanMap()
    {
        GameObject caveGO = GameObject.Find("Cave");

        var finishPrefab = Resources.Load<GameObject>("FinishPoint");


        var (xStart, yStart) = _caveBuilder.GetPlayerPosition();
        
        _playerMech.SetInitialPlace(xStart, yStart);
        _playerMech.gameObject.SetActive(true);

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

    private void PrepareResourcesPool() {
        GameObject poolsGO = GameObject.Find("Pools");
        var allResources = Resources.LoadAll<GameObject>("Dropables");
        var allIcons = Resources.LoadAll<Sprite>("Dropables");

        foreach (var onePrefab in allResources)
        {
            // var newPool = new GameObject(onePrefab.name);
            // newPool.transform.parent = poolsGO.transform;
            _mainStorage.ResPrefabs[onePrefab.name] = onePrefab;

            // var newLean = newPool.AddComponent<LeanGameObjectPool>();
            // newLean.Prefab = onePrefab;
            // newLean.Preload = 10;
            // newLean.Capacity = 30;
        }

        foreach(var oneSprite in allIcons) {
            var rgx = new Regex("-icon");
            var name = rgx.Replace(oneSprite.name, "");
            Debug.Log("ICON N: " + name);

            _mainStorage.ResSprites[name] = oneSprite;
        }

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
            return false;
        }

        var levelData = dataManager.LoadAsteroid(_minerState.AsteroidName);

        var cave = GameObject.Find("Cave");

        for (int i = cave.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(cave.transform.GetChild(i).gameObject);
        }

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

        var decorPrefabs = new Dictionary<string, GameObject>()
        {
            ["Crystal1"] = Resources.Load("Crystal1") as GameObject,
            ["Crystal2"] = Resources.Load("Crystal2") as GameObject,
        };

        var spawnPrefabs = new Dictionary<string, GameObject>()
        {
            ["EnemySpawner"] = Resources.Load("EnemySpawner") as GameObject,
        };

        var pillarPrefabs = new Dictionary<string, GameObject>()
        {
            ["Stolp"] = Resources.Load("Stolp") as GameObject,
        };

        foreach (BlockDto blockDto in levelData.Blocks)
        {
            var newBlock = Instantiate(blockPrefabs[blockDto.Type],
                new Vector3(blockDto.X, blockDto.Y, blockDto.Z),
                Quaternion.identity);
            var resourceBlock = newBlock.GetComponent<ResourceBlock>();

            var damagable = newBlock.GetComponent<Damagable>();
            damagable.CurrentLife = blockDto.Life;
            newBlock.transform.parent = cave.transform;

            resourceBlock.ChooseAppearance(blockDto.Life);
        }

        foreach (SegmentDto wallDto in levelData.Walls)
        {
            var newWall = Instantiate(wallPrefabs[wallDto.Type],
                new Vector3(wallDto.X, wallDto.Y, wallDto.Z),
                Quaternion.Euler(0, wallDto.YRotation, 0));

            newWall.transform.parent = cave.transform;
        };

        foreach (SegmentDto floorDto in levelData.Floors)
        {
            var newFloor = Instantiate(floorPrefabs[floorDto.Type],
                new Vector3(floorDto.X, floorDto.Y, floorDto.Z),
                Quaternion.Euler(0, floorDto.YRotation, 0));

            newFloor.transform.parent = cave.transform;
        };

        foreach (SegmentDto decorDto in levelData.Decorations)
        {
            var newDecor = Instantiate(decorPrefabs[decorDto.Type],
                new Vector3(decorDto.X, decorDto.Y, decorDto.Z),
                Quaternion.Euler(0, decorDto.YRotation, 0));

            newDecor.transform.parent = cave.transform;
        };

        foreach (SegmentDto spawnerDto in levelData.Spawners)
        {
            var newSpawner = Instantiate(spawnPrefabs[spawnerDto.Type],
                new Vector3(spawnerDto.X, spawnerDto.Y, spawnerDto.Z),
                Quaternion.Euler(0, spawnerDto.YRotation, 0));

            newSpawner.transform.parent = cave.transform;
        };

        foreach (SegmentDto pillarDto in levelData.Pillars)
        {
            var newPillar = Instantiate(pillarPrefabs[pillarDto.Type],
                new Vector3(pillarDto.X, pillarDto.Y, pillarDto.Z),
                Quaternion.Euler(0, pillarDto.YRotation, 0));

            newPillar.transform.parent = cave.transform;
        };

        return true;
    }

    private void LoadStorage()
    {
        var dataManager = new DataManager();

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

        _mainStorage.SetMechPartsDict(tmpParts);
    }

    private IEnumerator RescanNavigation()
    {
        yield return new WaitForEndOfFrame();

        _eventBus.ScanNavigationGrid?.Invoke();

        yield return new WaitForEndOfFrame();

        _eventBus.MapReady?.Invoke();
    }
}
