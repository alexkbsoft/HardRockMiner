using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceBlock : MonoBehaviour
{
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


    void Start()
    {
        _currentVisual = _block100;

        GetComponent<Damagable>().OnDamaged.AddListener(Damaged);
    }

    public void Destroyed()
    {
        for (int i = 0; i < ResourceCount; i++)
        {
            GenerateResource();
        }

        Destroy(gameObject);
    }

    public void Damaged(float livesRemain)
    {
        var nextVisual = livesRemain switch
        {
            var l when l > 75 => _block100,
            var l when l > 50 => _block75,
            var l when l > 25 => _block50,
            _ => _block25,
        };

        if (nextVisual != null && nextVisual != _currentVisual) {
            _currentVisual.SetActive(false);
            _currentVisual = nextVisual;
            nextVisual.SetActive(true);
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
            var resource = Instantiate(ResourcePrefab, transform.position + Vector3.up * 3, Quaternion.identity);
            resource.GetComponent<Resource>().SetType(DropRate[index].name);
            var direction = Random.onUnitSphere;
            direction.y = 0;
            resource.GetComponent<Rigidbody>().AddForce(direction * 10, ForceMode.Impulse);
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