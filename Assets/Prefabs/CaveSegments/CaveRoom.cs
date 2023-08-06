using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cave/Cave room")]

public class CaveRoom : ScriptableObject
{
    [SerializeField] public string[] RoomMap;
    [SerializeField] public KeyBlock[] KeyBlocks;
}

[System.Serializable]
public struct KeyBlock
{
    public int x;
    public int y;
    public GameObject block;
}
