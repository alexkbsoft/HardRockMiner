using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

[CreateAssetMenu(menuName = "Cave/Cave pattern")]

public class CavePatternSO : ScriptableObject
{
    public int SizeX;
    public int SizeY;
    //public CaveSegmet[] Pattern;
    public string[] Pattern;
    public int RandomGroups;
    public FixedRoom[] CaveRooms;
    public Deposit[] Deposits;
    public Deposit[] DepositsLayer2;
    public Deposit[] Wormholes;
    public Decoration[] Decorations;
    public int ColumnChance;
    public int SpawnersCount;
    public Vector2 StartPosition;
    public int[,] Map;

    public void Init()
    {
        Map = new int[SizeX, SizeY];
 
        for (int y = 0; y < SizeY; y++)
        {
            if (y < SizeY)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    if (x < Pattern[y].Length)
                    {
                        Map[x, y] = int.Parse(Pattern[y][x].ToString());
                    }
                    else
                    {
                        Map[x, y] = 1;
                    }
                }
            }
            else
            {
                for (int x = 0; x < SizeX; x++)
                {
                    Map[x, y] = 1;
                }
            }
        }
    }

    [Serializable]
    public struct CaveSegmet
    {
        public List<GameObject> SegmentVariants;
        public List<float> RotationVariants;
        public int RandomGroup;
    }

    [Serializable]
    public struct Deposit
    {
        public int Size;
        public int BlockIndex;
    }

    [Serializable]
    public struct Decoration
    {
        public int Count;
        public DecorationType Type;
    }

    [Serializable]
    public struct FixedRoom
    {
        public CaveRoom Room;
        public Point[] Positions;
    }

    [Serializable]
    public struct Point
    {
        public int x;
        public int y;
        public int rotation;
    }
}