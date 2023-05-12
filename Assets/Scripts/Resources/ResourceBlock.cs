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

    public void Destroyed()
    {
        for (int i = 0; i < ResourceCount; i++)
        {
            GenerateResource();
        }
        Destroy(gameObject);
    }

    private void GenerateResource()
    {
        var tmp = new float[DropRate.Length];
        for (int i = 0; i < DropRate.Length; i++)
        {
            tmp[i] = DropRate[i].rate;
        };

        int index = Chance.GetRandomTier(tmp);
        Debug.Log("INDEX: " + index);

        if (index != 0)
        {
            var resource = Instantiate(ResourcePrefab, transform.position + Vector3.up * 3, Quaternion.identity); 
            resource.GetComponent<Resource>().SetType(DropRate[index].name);
            resource.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 2, ForceMode.Impulse);
        }
    }

    void OnDestroy()
    {
        if (TryGetComponent<Damagable>(out var damagable))
        {
            damagable.OnDestroyed.RemoveListener(Destroyed);
        }
    }
}
