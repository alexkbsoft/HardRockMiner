using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField] MinerState _minerState;

    private EventBus _eventBus;
    private bool _haveActiveSpawner = false;

    void Start()
    {
        _minerState.Reset();

        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.AlarmChanged?.AddListener(OnAlarmChanged);   
        _eventBus.ActivateSpawner?.AddListener(SpawnerActivated);

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
    }

    private void SpawnerActivated(bool activated) {
        _haveActiveSpawner = true;
    }

    void OnDestroy()
    {
        _eventBus.AlarmChanged?.RemoveListener(OnAlarmChanged);
    }
}
