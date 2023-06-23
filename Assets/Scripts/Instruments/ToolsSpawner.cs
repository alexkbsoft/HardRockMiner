using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsSpawner : MonoBehaviour
{
    [SerializeField] GameObject MinePrefab;
    [SerializeField] GameObject LiserPrefab;
    [SerializeField] GameObject CirclePrefab;
    private EventBus _eventBus;

    void Start()
    {
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.ItemSpawn?.AddListener(SpawnItem);
    }

    private void SpawnItem(string id)
    {
        var playerPos = MechController.Instance.transform;
        // bool groundFound = Physics.Raycast(
        //     playerPos.position + playerPos.transform.forward * 4,
        //     Vector3.down,
        //     out var hitInfo,
        //     20,
        //     1 << LayerMask.NameToLayer("Ground"));

        // Debug.Log("SpawnItem: " + id);

        // if (!groundFound) {
        //     return;
        // }

        var pos = playerPos.position + playerPos.transform.forward * 4;

        switch (id)
        {
            case "mine":
                Instantiate(MinePrefab, pos, playerPos.transform.rotation);
                break;
            case "laser":
                Instantiate(LiserPrefab, pos, playerPos.transform.rotation);
                break;
            case "circular":
                Instantiate(CirclePrefab, pos, playerPos.transform.rotation);
                break;
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        _eventBus.ItemSpawn?.RemoveListener(SpawnItem);
    }
}
