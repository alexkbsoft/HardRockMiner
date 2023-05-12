using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    public MinerState State;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Resource")) {
            var res = other.gameObject.GetComponent<Resource>();
            State.Add(res);

            Destroy(other.gameObject);
        }        
    }
}
