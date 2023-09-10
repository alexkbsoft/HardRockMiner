using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cave/Resources List")]

public class RecourcesSO : ScriptableObject
{
    [SerializedDictionary("Block index","Block")] public SerializedDictionary<int, GameObject> BlockDictionary;

    public GameObject GetBlock(int index)
    {
        return BlockDictionary.GetValueOrDefault(index);
    }
}

public enum DecorationType
{
    Crystal
}
