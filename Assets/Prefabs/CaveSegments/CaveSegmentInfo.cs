using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CaveSegmentInfo : MonoBehaviour
{
    [SerializeField] public string[] FreeSpaceMap;
    [SerializeField] public string UniqName;

    public int RowSize = 10;
    private int[,] _caveInfo;

    public void Init(float rotation)
    {
        _caveInfo = new int[RowSize, RowSize];
        int rotationCount;

        for(int y=0; y< RowSize; y++)
        {
            if (y < FreeSpaceMap.Length)
            {
                for (int x = 0; x < RowSize; x++)
                {
                    if (x < FreeSpaceMap[y].Length)
                    {
                        _caveInfo[x, y] = int.Parse(FreeSpaceMap[y][x].ToString());
                    }
                    else
                    {
                        _caveInfo[x, y] = 1;
                    }
                }
            }
            else
            {
                for (int x = 0; x < RowSize; x++)
                {
                    _caveInfo[x, y] = 1;
                }
            }
        }

        rotationCount = Mathf.RoundToInt(rotation / 90);
        //rotationCount += 2;

        if (rotationCount > 0)
        {
            for (int i = 0; i < rotationCount; i++)
            {
                _caveInfo = RotateArray90(_caveInfo, _caveInfo.GetLength(0));
            }
        }
    }

    private int[,] RotateArray90(int[,] array, int size)
    {
        int[,] tempArray = new int[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                tempArray[x, y] = array[y, size-1-x];
            }
        }

        return tempArray;
    }

    public int[,] GetSegmentMap()
    {
        return _caveInfo;
    }
}
