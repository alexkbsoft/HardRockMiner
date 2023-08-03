using System.Collections.Generic;
using DataLayer;
using UnityEditor;
using UnityEngine;

namespace Storage
{
    [CreateAssetMenu(fileName = "MainStorage", menuName = "Create general store", order = 2)]
    public class MainStorage: ScriptableObject
    {
        public List<MinerState.StoredResource> resources;
        public List<ResourceDto> InventoryItems;
        public List<string> StackableItems;
        
        private Dictionary<string, string> _mechParts = new();

        public Dictionary<string, string> MechParts
        {
            get => _mechParts;
        }

        public void Add(MinerState.StoredResource res)
        {
            var existing = resources.Find((MinerState.StoredResource item) => item.name == res.name);

            if (existing != null)
            {
                existing.count += res.count;
            }
            else
            {
                resources.Add(new MinerState.StoredResource()
                {
                    name = res.name,
                    count = res.count
                });
            }
        }

        public void SetMechPart(string partName, string itemId)
        {
            _mechParts[partName] = itemId;
            EditorUtility.SetDirty(this);
        }

        public void SetMechPartsDict(Dictionary<string, string> newParts)
        {
            _mechParts = newParts;
            EditorUtility.SetDirty(this);
        }

        public ResourceDto FindFreeInventorySlot() {
            ResourceDto found = null;

            for (int i = 0; i < InventoryItems.Count; i++)
            {
                var res = InventoryItems[i];

                if (string.IsNullOrEmpty(res.name)) {
                    found = res;
                    
                    break;
                }
            }

            return found;
        }
    }
    
    [CustomEditor(typeof(MainStorage))]
    public class MainStorageEditor : Editor {

        MainStorage comp;
        static bool showTileEditor = false;

        public void OnEnable()
        {
            comp = (MainStorage)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            string formattedContents = "";

            foreach (var pairs in comp.MechParts)
            {
                formattedContents += $"{pairs.Key} = {pairs.Value}\n";
            }

            EditorGUILayout.HelpBox(formattedContents, MessageType.Info);
        }

    }
}