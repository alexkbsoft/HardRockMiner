using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pathfinding;

public class EnemySpawner : MonoBehaviour
{
    public float Period = 1.0f;
    public int WaveSize = 3;
    [SerializeField] private GameObject EnemyPrefab;

    private Coroutine _spawnCo;
    private bool _isActive = false;
    private EventBus _eventBus;

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
            StartSpawn();
        }
    }

    private IEnumerator SpawnEnemy()
    {
        int tmpSize = WaveSize;

        while (tmpSize > 0)
        {
            tmpSize--;
            yield return new WaitForSeconds(Period);
            var pos = transform.position + Random.onUnitSphere;
            pos.y = 0;

            var newEnemy = Instantiate(EnemyPrefab, pos, Quaternion.identity);

            var destination = newEnemy.GetComponent<AIDestinationSetter>();
            destination.target = MechController.Instance.transform;
        }
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
}
