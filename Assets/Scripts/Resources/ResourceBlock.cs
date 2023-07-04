using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;


public class ResourceBlock : MonoBehaviour
{
    public string BlockType;
    
    [System.Serializable]
    public struct Probability
    {
        public string name;
        public float rate;
    }
    public float ResourceCount = 1;
    public Probability[] DropRate;

    public GameObject ResourcePrefab;

    [SerializeField] private GameObject _block100;
    [SerializeField] private GameObject _block75;
    [SerializeField] private GameObject _block50;
    [SerializeField] private GameObject _block25;
    

    private GameObject _currentVisual;
    private EventBus _eventBus;
    
    private bool _shaking = false;

    void Awake()
    {
        _currentVisual = _block100;
    }

    void Start()
    {
        GetComponent<Damagable>().OnDamaged.AddListener(Damaged);
        
        _eventBus = FindObjectOfType<EventBus>();
    }

    public void Destroyed()
    {
        for (int i = 0; i < ResourceCount; i++)
        {
            GenerateResource();
        }

        _eventBus.BlockDestroyed?.Invoke(this);

        foreach (Transform part in _block25.transform)
        {
            part.transform.parent = null;
            part.GetComponent<WallPart>().Activate();
        }

        Destroy(gameObject);
    }

    public void Damaged(float livesRemain)
    {
        ChooseAppearance(livesRemain);
        _eventBus.BlockDamaged?.Invoke(this);

        Shake();
    }

    public void ChooseAppearance(float life)
    {
        var nextVisual = life switch
        {
            var l when l > 75 => _block100,
            var l when l > 50 => _block75,
            var l when l > 25 => _block50,
            _ => _block25,
        };

        if (nextVisual != null && nextVisual != _currentVisual)
        {
            _currentVisual.SetActive(false);
            _currentVisual = nextVisual;
            nextVisual.SetActive(true);
        }
        
    }

    private void Shake() {
        if (!_shaking) {
            _shaking = true;
            transform.DOShakePosition(0.5f, new Vector3(0.1f, 0, 0.1f)).OnComplete(() => _shaking = false);
        }
    }

    private void GenerateResource()
    {
        var tmp = new float[DropRate.Length];
        for (int i = 0; i < DropRate.Length; i++)
        {
            tmp[i] = DropRate[i].rate;
        };

        int index = Chance.GetRandomTier(tmp);

        if (index != 0)
        {
            var resource = Instantiate(ResourcePrefab,
                transform.position + Vector3.up * 3,
                Quaternion.identity);

            resource.GetComponent<Resource>().SetType(DropRate[index].name);
            var direction = Random.onUnitSphere;
            direction.y = 0;

            var rb = resource.GetComponent<Rigidbody>();
            rb.AddForce(direction * 10, ForceMode.Impulse);
            rb.AddTorque(direction * 3, ForceMode.Impulse);
        }
    }

    void OnDestroy()
    {
        if (TryGetComponent<Damagable>(out var damagable))
        {
            damagable.OnDestroyed.RemoveListener(Destroyed);
            damagable.OnDamaged.RemoveListener(Damaged);
        }
    }
}
