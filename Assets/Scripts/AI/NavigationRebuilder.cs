using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationRebuilder : MonoBehaviour
{
    [SerializeField] private AstarPath _astarPathBuilder;

    private EventBus _eventBus;
    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.BlockDestroyed?.AddListener(BlockDestroyed);   
    }

    private void BlockDestroyed(ResourceBlock block) {
        StartCoroutine(DelayedRebuild());
    }

    private IEnumerator DelayedRebuild() {
        yield return new WaitForEndOfFrame();

        _astarPathBuilder.Scan();
        Debug.Log("REBUILD SUCESS");
    }
}
