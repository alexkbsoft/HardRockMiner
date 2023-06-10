using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillActivator : MonoBehaviour
{
    [SerializeField] private LaserDrill _laserDrill;

    private EventBus _eventBus;
    void Start()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.DrillEnabled?.AddListener(OnEnableDrill);
    }

    public void OnEnableDrill(bool drilling)
    {
        _laserDrill.EnableDrill(drilling);
    }

    void OnDestroy()
    {
        _eventBus.DrillEnabled?.RemoveListener(OnEnableDrill);
    }
}
