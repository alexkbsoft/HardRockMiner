using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cave/Biome")]
public class BiomeSO : ScriptableObject
{
    [SerializeField] public GameObject FlatFloor;
    [SerializeField] public GameObject TrencedFloorFull;
    [SerializeField] public GameObject TrencedFloorHalf;
    [SerializeField] public GameObject[] Walls;
    [SerializeField] public GameObject Column;
}
