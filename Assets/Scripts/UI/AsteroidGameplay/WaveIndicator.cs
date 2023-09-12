using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveIndicator : MonoBehaviour
{
    [SerializeField] private Image ProgressCircle;
    [SerializeField] private GameObject InactiveIndicator;
    [SerializeField] private GameObject ActiveIndicator;
    [SerializeField] private TextMeshProUGUI _spawnersCountText;
    [SerializeField] private TextMeshProUGUI _alarmLevel;
    [SerializeField] private MinerState _minerState;

    private EventBus _eventBus;
    private int _spawnersCount = 0;
    private int _currentSpawning = 0;

    void Start()
    {
        _eventBus = EventBus.Instance;
        _eventBus.AlarmChanged?.AddListener(AlarmChange);
        _eventBus.AlarmInvoked?.AddListener(OnAlarm);
        _eventBus.ActivateSpawner?.AddListener(SpawnerActivated);
        _eventBus.WaveUnitsDead?.AddListener(SpawnerWaveEnd);
        
        ProgressCircle.fillAmount = _minerState.CurrentAlarm;
        ProgressCircle.gameObject.SetActive(false);
    }

    private void AlarmChange(float alarmDelta)
    {
        ProgressCircle.fillAmount = _minerState.CurrentAlarm;
    }

    private void OnAlarm(bool alarm)
    {
        InactiveIndicator.SetActive(!alarm);
        ActiveIndicator.SetActive(alarm);
        
        _alarmLevel.text = $"+{_minerState.AlarmLevel + 1}";
        
        _currentSpawning = _spawnersCount;
    }

    private void SpawnerWaveEnd()
    {
        _currentSpawning--;

        if (_currentSpawning == 0)
        {
            _minerState.EscalateAlarm();

            ProgressCircle.fillAmount = 0;
            
            InactiveIndicator.SetActive(true);
            ActiveIndicator.SetActive(false);
        }
    }

    private void SpawnerActivated(bool activated) {
        
        _spawnersCount ++;
        _spawnersCountText.text = $"{_spawnersCount}";


        ProgressCircle.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        _eventBus.AlarmChanged?.RemoveListener(AlarmChange);
        _eventBus.AlarmInvoked?.RemoveListener(OnAlarm);
        _eventBus.ActivateSpawner?.RemoveListener(SpawnerActivated);
    }
}
