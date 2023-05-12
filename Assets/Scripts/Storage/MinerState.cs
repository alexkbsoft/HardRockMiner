using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinerState", menuName = "Create store", order = 1)]
public class MinerState : ScriptableObject
{
    [System.Serializable]
    public class StoredResource {
        public string name;
        public int count;
    }

    public List<StoredResource> resources;

    public void Add(Resource res) {
        var existing = resources.Find((StoredResource item) => item.name == res.Name);

        if (existing != null) {
            existing.count += 1;
        } else {
            resources.Add(new StoredResource() {
                name = res.Name,
                count = 1
            });
        }
    }

}
