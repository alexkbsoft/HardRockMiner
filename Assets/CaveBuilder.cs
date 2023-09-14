using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class CaveBuilder : MonoBehaviour
{
    [SerializeField] private CavePatternSO _currentPattern;
    [SerializeField] private RecourcesSO _recourcesList;
    [SerializeField] private float _segmentSize;
    [SerializeField] private float _wallOffset;
    [SerializeField] private float _blockSize = 5;
    [SerializeField] private int _maxColumnCount = 5;
    [SerializeField] private int _maxDoubleColumnCount = 1;
    [SerializeField] private Text _testText;
    //[SerializeField] private RecourcesManager _recourcesManager;
    [SerializeField] private Vector3 _blockSpawnOffset;
    [SerializeField] private BiomeSO[] Biomes;

    private CavePatternSO[] _patterns;
    private Transform _cave;
    private int _row = 0;
    private int _segmentInRow = 0;
    private float[] _randomGroups;
    private int[,] _caveBlockMap;
    private int _blockCountX;
    private int _blockCountY;
    private const int segmentBlockCount = 5;
    private BiomeSO _currentBiome;

    private EventBus _eventBus;

    const int columnIndex = -10;
    const int generationAttempts = 100;
    // Start is called before the first frame update
    void Start()
    {
        _eventBus = EventBus.Instance;

        //TestFill();
        //Generate();
        if (_recourcesList == null)
        {
            return;
            Debug.Log("�� ���������� ���� ��������!!!");
        }
    }

    public (float, float) GetPlayerPosition() {
        var startPos = _currentPattern.StartPosition;

        return (startPos.x * _blockSize + _blockSpawnOffset.x,
            startPos.y * _blockSize + _blockSpawnOffset.y);
    }

    [Button]
    public void Generate()
    {
        GameObject caveGO = GameObject.Find("Cave");
        GameObject spawnersGO = GameObject.Find("Spawners");
        _cave = caveGO.transform;

        for (int i = _cave.childCount-1; i >= 0; i--)
        {
            DestroyImmediate(_cave.GetChild(i).gameObject);
        }
        for (int i = spawnersGO.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(spawnersGO.transform.GetChild(i).gameObject);
        }

        _currentBiome = Biomes[Random.Range(0, Biomes.Length)];
        _patterns = Resources.LoadAll<CavePatternSO>("CavePatterns");
        _currentPattern = _patterns[Random.Range(0, _patterns.Length)];
        //var _blockContainer = GameObject.Find("BlocksContainer");

        //if (_blockContainer != null)
        //{
        //    DestroyImmediate(_blockContainer);
        //}

        _blockCountX = _currentPattern.SizeX * segmentBlockCount;
        _blockCountY = _currentPattern.SizeY * segmentBlockCount;
        _caveBlockMap = new int[_blockCountX, _blockCountY];
        
        if (_currentPattern == null) return;
        _row = 0;
        _segmentInRow = 0;

        // if (_cave != null) DestroyImmediate(_cave.gameObject);
        _randomGroups = new float[_currentPattern.RandomGroups];

        _currentPattern.Init();

        for (int y = 0; y < _currentPattern.SizeY; y++)
        {
            for (int x = 0; x < _currentPattern.SizeX; x++)
            {
                GameObject newSegmnet;
                if (_currentPattern.Map[x, y] != 0)
                {
                    newSegmnet = new();
                }
                else 
                { 
                    FillSegmentSpace(x * segmentBlockCount, y * segmentBlockCount, null, -1);
                    continue;
                }
                newSegmnet.transform.SetParent(_cave);
                newSegmnet.name = "Segment-" + y.ToString() + "-" + x.ToString();
                newSegmnet.transform.SetAsLastSibling();


                if (_currentPattern.Map[x, y] == 1)
                {
                    int floorVariant = Random.Range(0, _currentBiome.FlatFloor.Length);
                    var newFloorSegment = Instantiate<GameObject>(_currentBiome.FlatFloor[floorVariant], newSegmnet.transform);
                    newFloorSegment.transform.position = new Vector3(x * _segmentSize, 0, y * _segmentSize);
                }

                FillSegmentSpace(x * segmentBlockCount, y * segmentBlockCount, null);


                if (x==0||_currentPattern.Map[x-1, y] == 0)
                {
                    int wallIndex = Random.Range(0, _currentBiome.Walls.Length);
                    var newWall = Instantiate<GameObject>(_currentBiome.Walls[wallIndex], newSegmnet.transform);
                    newWall.transform.position = new Vector3(x * _segmentSize - _segmentSize/2 - _wallOffset, 0, y * _segmentSize);
                    newWall.transform.rotation = Quaternion.Euler(0,0,0);
                }

                if (x == _currentPattern.SizeX-1 || _currentPattern.Map[x + 1, y] == 0)
                {
                    int wallIndex = Random.Range(0, _currentBiome.Walls.Length);
                    var newWall = Instantiate<GameObject>(_currentBiome.Walls[wallIndex], newSegmnet.transform);
                    newWall.transform.position = new Vector3(x * _segmentSize + _segmentSize / 2 + _wallOffset, 0, y * _segmentSize);
                    newWall.transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                if (y == 0 || _currentPattern.Map[x, y - 1] == 0)
                {
                    int wallIndex = Random.Range(0, _currentBiome.Walls.Length);
                    var newWall = Instantiate<GameObject>(_currentBiome.Walls[wallIndex], newSegmnet.transform);
                    newWall.transform.position = new Vector3(x * _segmentSize, 0, y * _segmentSize - _segmentSize / 2 - _wallOffset);
                    newWall.transform.rotation = Quaternion.Euler(0, 270, 0);
                }

                if (y == _currentPattern.SizeY - 1 || _currentPattern.Map[x, y + 1] == 0)
                {
                    int wallIndex = Random.Range(0, _currentBiome.Walls.Length);
                    var newWall = Instantiate<GameObject>(_currentBiome.Walls[wallIndex], newSegmnet.transform);
                    newWall.transform.position = new Vector3(x * _segmentSize, 0, y * _segmentSize + _segmentSize / 2 + _wallOffset);
                    newWall.transform.rotation = Quaternion.Euler(0, 90, 0);
                }

                for (int i = 0; i < _maxDoubleColumnCount; i++)
                {
                    if (Random.Range(0, 100) <= _currentPattern.DoubleColumnChance)
                    {
                        int columnX = 0;
                        int columnY = 0;
                        int attempt = 0;
                        bool columnPositionValid = false;
                        while (attempt < 100 && !columnPositionValid)
                        {
                            columnX = Random.Range(1, segmentBlockCount - 2);
                            columnY = Random.Range(1, segmentBlockCount - 2);
                            columnPositionValid = true;
                            if (CheckBlockPersist(x * segmentBlockCount + columnX, y * segmentBlockCount + columnY, 2, columnIndex))
                                columnPositionValid = false;
                            attempt++;
                        }
                        if (!columnPositionValid) continue;
                        var newColumn = Instantiate<GameObject>(_currentBiome.DoubleColumn, newSegmnet.transform);
                        newColumn.transform.position = new Vector3(_blockSpawnOffset.x + x * _segmentSize + columnX * _blockSize,
                                                                    0,
                                                                    _blockSpawnOffset.y + y * _segmentSize + columnY * _blockSize);
                        int columnRotation = Random.Range(0, 4);
                        newColumn.transform.rotation = Quaternion.Euler(0, 90 * columnRotation, 0);
                        _caveBlockMap[x * segmentBlockCount + columnX, y * segmentBlockCount + columnY] = columnIndex;
                        switch (columnRotation)
                        {
                            case 0: _caveBlockMap[x * segmentBlockCount + columnX + 1, y * segmentBlockCount + columnY] = columnIndex;
                                break;
                            case 1: _caveBlockMap[x * segmentBlockCount + columnX, y * segmentBlockCount + columnY - 1] = columnIndex;
                                break;
                            case 2: _caveBlockMap[x * segmentBlockCount + columnX - 1, y * segmentBlockCount + columnY] = columnIndex;
                                break;
                            case 3: _caveBlockMap[x * segmentBlockCount + columnX, y * segmentBlockCount + columnY + 1] = columnIndex;
                                break;
                        }
                    }
                }

                for (int i = 0; i < _maxColumnCount; i++)
                {
                    if (Random.Range(0,100)<=_currentPattern.ColumnChance) {
                        int columnX=0;
                        int columnY=0;
                        int attempt = 0;
                        bool columnPositionValid = false;
                        while (attempt<100&&!columnPositionValid)
                        {
                            columnX = Random.Range(0, segmentBlockCount - 1);
                            columnY = Random.Range(0, segmentBlockCount - 1);
                            columnPositionValid = true;
                            if(CheckBlockPersist(x * segmentBlockCount + columnX, y * segmentBlockCount + columnY,2,columnIndex)) 
                                columnPositionValid=false;
                            attempt++;
                        }
                        if (!columnPositionValid) continue;
                        var newColumn = Instantiate<GameObject>(_currentBiome.Column, newSegmnet.transform);
                        newColumn.transform.position = new Vector3( _blockSpawnOffset.x+ x * _segmentSize + columnX * _blockSize ,
                                                                    0,
                                                                    _blockSpawnOffset.y+ y * _segmentSize + columnY * _blockSize);
                        int columnRotation = Random.Range(0, 4);
                        newColumn.transform.rotation = Quaternion.Euler(0, 90*columnRotation, 0);
                        _caveBlockMap[x * segmentBlockCount + columnX, y * segmentBlockCount + columnY] = columnIndex;
                    }
                }
            }
        }

        CaveFillerModel caveFiller = new CaveFillerModel(_currentPattern, segmentBlockCount);
        _caveBlockMap = caveFiller.Init(_caveBlockMap,_currentBiome);
        SpawnBlocks();
    }

    private void FillSegmentSpace(int offsetX, int offsetY, int[,] segmentMap, int index=0)
    {
        for (int y = 0; y < segmentBlockCount; y++)
        {
            for (int x = 0; x < segmentBlockCount; x++)
            {
                int currentBlockContent = -1;
                if (segmentMap != null)
                {
                    currentBlockContent = segmentMap[x, y];
                }
                else
                {
                    currentBlockContent = 1;
                }
                if (index!=0)
                {
                    _caveBlockMap[offsetX + x, offsetY + y] = index;
                }
                else
                {
                    _caveBlockMap[offsetX + x, offsetY + y] = currentBlockContent;
                }
            }
        }
    }

    private bool CheckBlockPersist(int startX, int startY, int radius, int blockIndex)
    {
        for (int y = startY - radius; y <= startY + radius; y++)
        {
            if (y < 0) continue;
            if (y > _currentPattern.SizeY* segmentBlockCount - 1) continue;
            for (int x = startX - radius; x <= startX + radius; x++)
            {
                if (x < 0) continue;
                if (x > _currentPattern.SizeX* segmentBlockCount - 1) continue;
                if (_caveBlockMap[x, y] == blockIndex) return true;
            }
        }

        return false;
    }

    private void SpawnBlocks()
    {
        if (_recourcesList == null)
        {
            return;
        }

        GameObject blockContainer = new GameObject("BlocksContainer");
        var spawnParent = GameObject.Find("Spawners");
        blockContainer.transform.SetParent(_cave);
        for (int y = 0; y < _blockCountY; y++)
        {
            for (int x = 0; x < _blockCountX; x++)
            {
                if (_caveBlockMap[x, y] > 0)
                {
                    var currentBlock = _recourcesList.GetBlock(_caveBlockMap[x, y]);
                    if (currentBlock == null) continue;

                    var currentParent = blockContainer.transform;
                    if (currentBlock.TryGetComponent<EnemySpawner>(out _)) 
                    {
                        currentParent = spawnParent.transform;
                    }
                    GameObject newBlock = Instantiate(currentBlock, currentParent);
                    int randomRotationX = Random.Range(0, 4);
                    int randomRotationY = Random.Range(0, 4);
                    int randomRotationZ = Random.Range(0, 4);
                    if (currentBlock.TryGetComponent<CaveDecoration>(out _))
                    {
                        randomRotationX = 0;
                        randomRotationZ = 0;
                    }
                    if (currentBlock.TryGetComponent<CaveBlockInfo>(out var blockInfo))
                    {
                        if (blockInfo.Fixed)
                        {
                            randomRotationX = 0;
                            randomRotationY = 0;
                            randomRotationZ = 0;
                        }
                    }
                    newBlock.transform.rotation = Quaternion.Euler(90f * randomRotationX, 90f*randomRotationY, 90f * randomRotationZ);
                    newBlock.transform.position = new Vector3(_blockSpawnOffset.x + x * _blockSize, _blockSpawnOffset.z, _blockSpawnOffset.y + y * _blockSize);
                    //newBlock.transform.position = new Vector3(_blockSpawnOffset.y + y * _blockSize, _blockSpawnOffset.z, _blockSpawnOffset.x + x * _blockSize);
                }
            }
        }

        if(_eventBus) _eventBus.MapGenerationDone?.Invoke();

        //StartCoroutine(CutPlayer());
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
            DestroyImmediate(hit.collider.gameObject);
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
