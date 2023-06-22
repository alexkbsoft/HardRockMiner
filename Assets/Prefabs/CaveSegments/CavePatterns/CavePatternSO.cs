using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

[CreateAssetMenu(menuName = "Cave pattern")]

public class CavePatternSO : ScriptableObject
{
    public Vector2 Size;
    public CaveSegmet[] Pattern;
    public int RandomGroups;

    [Serializable]
    public struct CaveSegmet
    {
        public List<GameObject> SegmentVariants;
        public List<float> RotationVariants;
        public int RandomGroup;
    }
}
