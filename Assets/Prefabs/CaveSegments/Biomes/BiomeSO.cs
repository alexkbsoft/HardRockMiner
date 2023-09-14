using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "Cave/Biome")]
public class BiomeSO : ScriptableObject
{
    [SerializeField] public GameObject[] FlatFloor;
    [SerializeField] public GameObject TrencedFloorFull;
    [SerializeField] public GameObject TrencedFloorHalf;
    [SerializeField] public GameObject[] Walls;
    [SerializedDictionary("Decor type", "Block")] public SerializedDictionary<DecorationType, int[]> Decorations;
    [SerializeField] public GameObject Column;
    [SerializeField] public GameObject DoubleColumn;
    [SerializeField] public GameObject TripleColumn;
}
