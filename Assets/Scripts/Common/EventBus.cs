using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public UnityEvent<List<GameObject>> TargetsChanged;
    public UnityEvent<ResourceBlock> BlockDestroyed;

    public UnityEvent<ResourceBlock> BlockDamaged;
    public UnityEvent<Resource> ResourceCollected;
    public UnityEvent<bool> FireEnabled;
    public UnityEvent<bool> DrillEnabled;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
