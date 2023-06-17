using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveIndicator : MonoBehaviour
{
    [SerializeField] private Image ProgressCircle;
    [SerializeField] private GameObject InactiveIndicator;
    [SerializeField] private GameObject ActiveIndicator;
    
    [SerializeField] private MinerState _minerState;

    private EventBus _eventBus;

    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.AlarmChanged?.AddListener(AlarmChange);

        ProgressCircle.fillAmount = _minerState.CurrentAlarm;
    }

    private void AlarmChange(float alarmDelta)
    {
        ProgressCircle.fillAmount = _minerState.CurrentAlarm;

        if (_minerState.CurrentAlarm >= 1) {
            InactiveIndicator.SetActive(false);
            ActiveIndicator.SetActive(true);
        }
    }

    void OnDestroy()
    {
        _eventBus.AlarmChanged?.RemoveListener(AlarmChange);
    }
}
