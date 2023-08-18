using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class RecourcesManager : MonoBehaviour
{
    [SerializedDictionary("Block index","Block")] public SerializedDictionary<int, GameObject> BlockDictionary;

    public GameObject GetBlock(int index)
    {
        return BlockDictionary.GetValueOrDefault(index);
    }
}
