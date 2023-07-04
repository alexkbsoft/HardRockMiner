using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinerState", menuName = "Create store", order = 1)]
public class MinerState : ScriptableObject
{
    public float MaximumAlarm = 100;
    public string AsteroidName = "default";
    
    [System.Serializable]
    public class StoredResource
    {
        public string name;
        public int count;
    }
    public List<StoredResource> resources;
    public float CurrentAlarm = 0;
    public int AlarmLevel = 0;

    public void Add(Resource res)
    {
        var existing = resources.Find((StoredResource item) => item.name == res.Name);

        if (existing != null)
        {
            existing.count += 1;
        }
        else
        {
            resources.Add(new StoredResource()
            {
                name = res.Name,
                count = 1
            });
        }
    }

    public void AddAlarm(float alarmDelta)
    {
        CurrentAlarm += alarmDelta / MaximumAlarm;
    }

    public void EscalateAlarm()
    {
        CurrentAlarm = 0;
        AlarmLevel++;
    }

    public void Reset()
    {
        resources.Clear();
        CurrentAlarm = 0;
        AlarmLevel = 0;
    }

}
