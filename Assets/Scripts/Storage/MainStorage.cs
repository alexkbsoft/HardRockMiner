using System.Collections.Generic;
using UnityEngine;

namespace Storage
{
    [CreateAssetMenu(fileName = "MainStorage", menuName = "Create general store", order = 2)]
    public class MainStorage: ScriptableObject
    {
        public List<MinerState.StoredResource> resources;
        
        public void Add(MinerState.StoredResource res)
        {
            Debug.Log("Find: " + res.name);
            var existing = resources.Find((MinerState.StoredResource item) => item.name == res.name);

            if (existing != null)
            {
                Debug.Log("Found!");
                existing.count += res.count;
            }
            else
            {
                Debug.Log("Not Found!");
                resources.Add(new MinerState.StoredResource()
                {
                    name = res.name,
                    count = res.count
                });
            }
        }
    }
}