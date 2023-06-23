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

    [SerializeField] private MinerState _minerState;


    private EventBus _eventBus;
    private int _spawnersCount = 0;

    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.AlarmChanged?.AddListener(AlarmChange);
        _eventBus.AlarmInvoked?.AddListener(OnAlarm);
        _eventBus.ActivateSpawner?.AddListener(SpawnerActivated);



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
    }

    private void SpawnerActivated(bool activated) {
        ProgressCircle.gameObject.SetActive(true);
        _spawnersCount++;
        _spawnersCountText.text = $"{_spawnersCount}";
    }

    void OnDestroy()
    {
        _eventBus.AlarmChanged?.RemoveListener(AlarmChange);
        _eventBus.AlarmInvoked?.RemoveListener(OnAlarm);
        _eventBus.ActivateSpawner?.RemoveListener(SpawnerActivated);
    }
}
