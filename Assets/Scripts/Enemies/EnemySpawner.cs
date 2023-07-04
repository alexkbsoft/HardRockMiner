using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Pathfinding;

public class EnemySpawner : MonoBehaviour
{
    public float Period = 1.0f;
    [SerializeField] private WaveDescriptor[] _waveLevels;
    [SerializeField] private MinerState _minerState;

    private Coroutine _spawnCo;
    private bool _isActive = false;
    private EventBus _eventBus;

    private int UnitsAlife = 0;
    
    [System.Serializable]
    public class WaveDescriptor
    {
        public WaveContent[] Content;
    }
    
    [System.Serializable]
    public class WaveContent
    {
        public GameObject EnemyPrefab;
        public int UnitsCount = 1;
    }

    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.AlarmInvoked?.AddListener(AlarmHappend);

        StartCoroutine(CheckPlayerReach());
    }

    [ContextMenu("Start spawn")]
    public void StartSpawn()
    {
        if (_spawnCo == null && _isActive)
        {
            _spawnCo = StartCoroutine(SpawnEnemy());
        }
    }

    [ContextMenu("Stop spawn")]
    public void StopSpawn()
    {
        if (_spawnCo != null)
        {
            StopCoroutine(_spawnCo);
            _spawnCo = null;
        }
    }

    private void AlarmHappend(bool alarm) {
        if (alarm) {
            Debug.Log("Alarm Start!");
            StartSpawn();
        }
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForEndOfFrame();

        var alarmLevel = _minerState.AlarmLevel;

        if (_minerState.AlarmLevel > _waveLevels.Length - 1 ||
            _waveLevels[alarmLevel].Content == null ||
            _waveLevels[alarmLevel].Content.Length == 0)
        {
            _eventBus.WaveUnitsDead?.Invoke();
            _spawnCo = null;

            yield break;
        }
        
        var waveDescriptors = _waveLevels[alarmLevel];

        UnitsAlife = waveDescriptors.Content
            .Aggregate(0, (acc, waveContnt) => waveContnt.UnitsCount);
        
        Debug.Log("Units: " + UnitsAlife);
        
        foreach (var oneContent in waveDescriptors.Content)
        {
            int tmpSize = oneContent.UnitsCount;
            GameObject _enemyPrefab = oneContent.EnemyPrefab;

            while (tmpSize > 0)
            {
                tmpSize--;
                yield return new WaitForSeconds(Period);
                var pos = transform.position + Random.onUnitSphere;
                pos.y = 0;
        
                var newEnemy = Instantiate(_enemyPrefab, pos, Quaternion.identity);
        
                var destination = newEnemy.GetComponent<AIDestinationSetter>();
                destination.target = MechController.Instance.transform;
                
                newEnemy.GetComponent<Damagable>().OnDestroyed.AddListener(DecrementUnits);
            }
        }
        
        _spawnCo = null;
    }

    private IEnumerator CheckPlayerReach()
    {
        while (!_isActive)
        {
            yield return new WaitForSeconds(1);

            GraphNode node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
            GraphNode node2 = AstarPath.active.GetNearest(MechController.Instance.transform.position, NNConstraint.Default).node;

            if (PathUtilities.IsPathPossible(node1, node2))
            {
                _isActive = true;
                _eventBus.ActivateSpawner?.Invoke(true);
            }
        }
    }

    private void DecrementUnits()
    {
        UnitsAlife--;

        if (UnitsAlife <= 0)
        {
            _eventBus.WaveUnitsDead?.Invoke();
        }
    }
}
