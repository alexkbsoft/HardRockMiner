using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cave/Cave room")]

public class CaveRoom : ScriptableObject
{
    [SerializeField] public string[] RoomMap;
    private int[,] _roomMap;

    public void Init(int rotation)
    {
        int rowCount = RoomMap.Length;
        if (rowCount == 0) return;
        int rowSize = RoomMap[0].Split(',').Length ;
        if (rowSize == 0) return;
        _roomMap = new int[rowSize, rowCount];
        int rotationCount;

        for (int y = 0; y < rowCount; y++)
        {
            string[] row = RoomMap[y].Split(',');
            for (int x = 0; x < rowSize; x++)
            {
                if (x < row.Length)
                {
                    _roomMap[x, y] = int.Parse(row[x]);
                }
                else
                {
                    _roomMap[x, y] = 1;
                }
            }
        }
        rotationCount = Mathf.RoundToInt(rotation / 90);
        //rotationCount += 2;

        if (rotationCount > 0)
        {
            for (int i = 0; i < rotationCount; i++)
            {
                _roomMap = RotateArray90(_roomMap, rowSize, rowCount);
                int t = rowSize;
                rowSize = rowCount;
                rowCount = t;
            }
        }
    }

    private int[,] RotateArray90(int[,] array, int sizeX, int sizeY)
    {
        int[,] tempArray = new int[sizeY, sizeX];
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                tempArray[x, y] = array[y, sizeX - 1 - x];
            }
        }
        return tempArray;
    }

    public int[,] GetRoom()
    {
        return _roomMap;
    }
}

