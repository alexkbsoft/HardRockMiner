using System.Collections;
using System.Collections.Generic;
using DataLayer;
using Storage;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] MinerState _minerState;
    [SerializeField] MainStorage _mainStorage;
    [SerializeField] private bool _loadOnStart = true; 

    private EventBus _eventBus;
    private bool _haveActiveSpawner = false;

    private static GameManager _instance;

    public static GameManager Instance => _instance;

    void Start()
    {
        _instance = this;
        
        _minerState.Reset();

        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.AlarmChanged?.AddListener(OnAlarmChanged);
        _eventBus.ActivateSpawner?.AddListener(SpawnerActivated);

        LoadStorage();
        
        _eventBus.DataReady?.Invoke();

        if (_loadOnStart)
        {
            LoadLevel();
        }
    }

    [ContextMenu("Save asteroid")]
    public void SaveAsteroid()
    {
        var dataManager = new DataManager();
        dataManager.SaveCurrentAsteroid(_minerState.AsteroidName);

        SceneManager.LoadScene("ResultScreen");
    }

    private void OnAlarmChanged(float delta) {
        if (!_haveActiveSpawner) {
            return;
        }

        var currntAlarm = _minerState.CurrentAlarm;

        if (currntAlarm < 1) {
            _minerState.AddAlarm(delta);

            if (_minerState.CurrentAlarm >= 1) {
                _eventBus.AlarmInvoked?.Invoke(true);
            }
        }
        
        if (currntAlarm > 1)
        {
            _minerState.CurrentAlarm = 1;
        }
    }

    private void SpawnerActivated(bool activated) {
        _haveActiveSpawner = true;
    }



    void OnDestroy()
    {
        _eventBus.AlarmChanged?.RemoveListener(OnAlarmChanged);
        _eventBus.ActivateSpawner?.RemoveListener(SpawnerActivated);
    }

    private void LoadLevel()
    {
        var dataManager = new DataManager();
        if (!dataManager.IsAsteroidExists(_minerState.AsteroidName))
        {
            return;
        }
        
        var levelData = dataManager.LoadAsteroid(_minerState.AsteroidName);

        var walls = GameObject.Find("Walls");
        
        foreach (Transform child in walls.transform) {
            GameObject.Destroy(child.gameObject);
        }

        var blockPrefabs = new Dictionary<string, GameObject>()
        {
            ["DirtBlock"] = Resources.Load("DirtBlock") as GameObject,
            ["BrickBlock"] = Resources.Load("BrickBlock") as GameObject,
            ["RockBlock"] = Resources.Load("RockBlock") as GameObject,
        };
        
        foreach (BlockDto blockDto in levelData.Blocks)
        {
            var newBlock = Instantiate(blockPrefabs[blockDto.Type],
                new Vector3(blockDto.X, 0.216f, blockDto.Z),
                Quaternion.identity);
            var resourceBlock = newBlock.GetComponent<ResourceBlock>();
            

            var damagable = newBlock.GetComponent<Damagable>();
            damagable.CurrentLife = blockDto.Life;
            newBlock.transform.parent = walls.transform;
            
            resourceBlock.ChooseAppearance(blockDto.Life);
        }

        StartCoroutine(RescanNavigation());
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

        foreach (string itemId in storageDto.Inventory)
        {
            _mainStorage.InventoryItems.Add(itemId);
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
    }
}
