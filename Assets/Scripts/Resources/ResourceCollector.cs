using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    public MinerState State;
    
    private EventBus _eventBus;
    void Start()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Resource")) {
            var res = other.gameObject.GetComponent<Resource>();
            State.Add(res);
            _eventBus.ResourceCollected?.Invoke(res);

            Destroy(other.gameObject);
        }        
    }
}
