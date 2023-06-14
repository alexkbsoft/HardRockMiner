using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationRebuilder : MonoBehaviour
{
    [SerializeField] private AstarPath _astarPathBuilder;

    private EventBus _eventBus;
    private Coroutine _corutine;
    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.BlockDestroyed?.AddListener(BlockDestroyed);   
    }

    private void BlockDestroyed(ResourceBlock block) {
        if (_corutine == null) {
            _corutine = StartCoroutine(DelayedRebuild());
        }
    }

    private IEnumerator DelayedRebuild() {
        yield return new WaitForSeconds(1.0f);

        _astarPathBuilder.Scan();
        _corutine = null;
        
        Debug.Log("REBUILD SUCESS");
    }
}
