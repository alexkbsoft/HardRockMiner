using System;
using System.Collections;
using System.Collections.Generic;
using DataLayer;
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
    public UnityEvent<string> ItemSpawn;
    public UnityEvent<float> AlarmChanged;
    public UnityEvent<bool> AlarmInvoked;
    public UnityEvent<bool> ActivateSpawner;
    public UnityEvent ScanNavigationGrid;
    public UnityEvent WaveUnitsDead;
    public UnityEvent<Draggable> DraggableTapped;
    public UnityEvent InventoryReordered;
    public UnityEvent<InventoryItem> DroppedInCraft;
    public UnityEvent<List<int>, string> SchemaDropped;
    public UnityEvent SchemaReset;
    public UnityEvent CraftItemRemoved;
    public UnityEvent<string, string> DroppedInMech;
    public UnityEvent<int> InventoryTabSelected;
    public UnityEvent UpdateMechStructure;
    public UnityEvent ResourcesUpdated;
    public UnityEvent MapReady;
    public UnityEvent MapGenerationDone;

    public UnityEvent DataReady;

    public static EventBus Instance;

    void Awake() {
        Instance = this;
    }
}
