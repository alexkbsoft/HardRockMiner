using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(RecourcesManager))]

public class CaveBuilder : MonoBehaviour
{
    [SerializeField] private CavePatternSO _currentPattern;
    [SerializeField] private float _segmentSize;
    [SerializeField] private float _blockSize = 5;
    [SerializeField] private Text _testText;
    [SerializeField] private RecourcesManager _recourcesManager;
    [SerializeField] private Vector3 _blockSpawnOffset;

    private Transform _cave;
    private int _row = 0;
    private int _segmentInRow = 0;
    private float[] _randomGroups;
    private int[,] _caveBlockMap;
    private int _blockCountX;
    private int _blockCountY;
    private const int segmentBlockCount = 10;

    private EventBus _eventBus;


    // Start is called before the first frame update
    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();

        //TestFill();
        //Generate();
        if (_recourcesManager == null) _recourcesManager = GetComponent<RecourcesManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Button]
    public void Generate()
    {
        var _blockContainer = GameObject.Find("BlocksContainer");

        if (_blockContainer != null)
        {
            DestroyImmediate(_blockContainer);
        }

        _blockCountX = Mathf.RoundToInt(_currentPattern.Size.x * segmentBlockCount);
        _blockCountY = Mathf.RoundToInt(_currentPattern.Size.y * segmentBlockCount);
        _caveBlockMap = new int[_blockCountX, _blockCountY];
        
        if (_currentPattern == null) return;
        _row = 0;
        _segmentInRow = 0;

        // if (_cave != null) DestroyImmediate(_cave.gameObject);
        
        GameObject caveGO = GameObject.Find("Cave");
        _cave = caveGO.transform;

        _randomGroups = new float[_currentPattern.RandomGroups];
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
                if (newSegment.TryGetComponent<CaveSegmentInfo>(out var segmentInfo))
                {
                    float rotation = 0f;
                    if (segment.RotationVariants.Count > 0) rotation = segment.RotationVariants[rotationVariant];
                    segmentInfo.Init(rotation);
                    FillSegmentSpace(_segmentInRow * segmentBlockCount, _row * segmentBlockCount, segmentInfo.GetSegmentMap());
                }
                else
                {
                    FillSegmentSpace(_segmentInRow * segmentBlockCount, _row * segmentBlockCount, null);
                }
            }
            else
            {

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

        CaveFillerModel caveFiller = new CaveFillerModel(_currentPattern, 10);
        _caveBlockMap = caveFiller.Init(_caveBlockMap);
        // _testText.text = caveFiller.Fill();
        SpawnBlocks();
    }

    private void FillSegmentSpace(int offsetX, int offsetY, int[,] segmentMap)
    {
        for (int y = 0; y < segmentBlockCount; y++)
        {
            for (int x = 0; x < segmentBlockCount; x++)
            {
                int currentBlockContent = 0;
                if (segmentMap != null)
                {
                    currentBlockContent = segmentMap[x, y];
                }
                _caveBlockMap[offsetX + x, offsetY + y] = currentBlockContent;
            }
        }
    }

    private void SpawnBlocks()
    {
        GameObject blockContainer = new GameObject("BlocksContainer");
        blockContainer.transform.SetParent(_cave);
        for (int y = 0; y < _blockCountY; y++)
        {
            for (int x = 0; x < _blockCountX; x++)
            {
                if (_caveBlockMap[x, y] != 0)
                {
                    GameObject newBlock = Instantiate(_recourcesManager.GetBlock(_caveBlockMap[x, y]), blockContainer.transform);
                    newBlock.transform.rotation = Quaternion.EulerAngles(0, 0, 0);
                    //newBlock.transform.position = new Vector3(_blockSpawnOffset.x + x * _blockSize, _blockSpawnOffset.z, _blockSpawnOffset.y - y * _blockSize);
                    newBlock.transform.position = new Vector3(_blockSpawnOffset.y + y * _blockSize, _blockSpawnOffset.z, _blockSpawnOffset.x + x * _blockSize);
                }
            }
        }

        StartCoroutine(CutPlayer());
    }

    private IEnumerator CutPlayer()
    {
        yield return new WaitForSeconds(0.5f);

        CutHole(Constants.LevelOrigin);

        var spawnParent = GameObject.Find("Spawners");
        var spawnPrefab = Resources.Load<GameObject>("EnemySpawner");

        for (int i = 0; i < 3; i++)
        {
            var one = Vector3.one;
            one.x *= Random.Range(25, 150);
            one.z *= Random.Range(75, 120);
            one.y = 0.47f;

            var spawner = Instantiate(spawnPrefab);
            spawner.transform.parent = spawnParent.transform;
            spawner.transform.position = one;

            CutHole(one);
        }

        _eventBus.MapGenerationDone?.Invoke();
    }

    private void CutHole(Vector3 xyPos)
    {
        float cutRadius = 15.0f;

        RaycastHit[] cubes = Physics.SphereCastAll(
            xyPos + Vector3.up * 50,
            cutRadius,
            Vector3.down,
            100,
            1 << LayerMask.NameToLayer("Walls"));

        foreach (var hit in cubes)
        {
            Destroy(hit.collider.gameObject);
        }
    }


    //[Button]
    //private void TestFill()
    //{
    //    CaveFillerModel caveFiller = new CaveFillerModel(_currentPattern);
    //    caveFiller.Init(null);
    //    _testText.text = caveFiller.Fill();
    //}
}
