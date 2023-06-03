using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavigationRebuilder : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _surface;

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

        _surface?.BuildNavMesh();
    }
}
