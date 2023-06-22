using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CaveBuilder : MonoBehaviour
{
    [SerializeField] private CavePatternSO _currentPattern;
    [SerializeField] private Transform _cave;
    [SerializeField] private float _segmentSize;
    private int _row=0;
    private int _segmentInRow=0;
    private float[] randomGroups;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    private void Generate()
    {
        if (_currentPattern == null) return;
        _row = 0;
        _segmentInRow = 0;
        if (_cave != null) DestroyImmediate(_cave.gameObject);
        GameObject caveGO = new GameObject();
        _cave = caveGO.transform;
        randomGroups = new float[_currentPattern.RandomGroups];
        foreach (CavePatternSO.CaveSegmet segment in _currentPattern.Pattern)
        {
            if (segment.SegmentVariants.Count > 0)
            {
                int segmentVariant = 0;
                int rotationVariant = 0;
                if (segment.RandomGroup == 0)
                {
                    segmentVariant = Random.RandomRange(0, segment.SegmentVariants.Count);
                    rotationVariant = Random.RandomRange(0, segment.RotationVariants.Count);
                }
                else
                {
                    
                }
                var newSegment = Instantiate<GameObject>(segment.SegmentVariants[segmentVariant], _cave);
                newSegment.transform.position = new Vector3(_row * _segmentSize, 0, _segmentInRow * _segmentSize);
                if (segment.RotationVariants.Count > 0)
                        newSegment.transform.rotation = Quaternion.Euler(0, segment.RotationVariants[rotationVariant], 0);
            };
            if (_segmentInRow == _currentPattern.Size.x - 1)
            {
                _segmentInRow = 0;
                _row++;
            }
            else
            {
                _segmentInRow++;
            }
        }
    }
}
