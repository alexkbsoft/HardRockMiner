using System;
using System.Collections.Generic;
using DataLayer;
using UnityEditor;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Lean.Pool;

namespace Storage
{
    [Serializable]
    public class RecipieItem
    {
        public string Id;
        public List<string> Resources = new();
    }

    [CreateAssetMenu(fileName = "MainStorage", menuName = "Create general store", order = 2)]
    public class MainStorage : ScriptableObject
    {
        public List<MinerState.StoredResource> resources;
        public List<ResourceDto> InventoryItems;
        public List<string> StackableItems;
        public List<RecipieItem> Recipies = new();
        public Dictionary<string, string> _mechParts = new();
        public Dictionary<string, GameObject> ResPrefabs = new();
        public Dictionary<string, Sprite> ResSprites = new();
        [SerializedDictionary("Item ID", "Description")] public SerializedDictionary<string, ItemDescription> ItemDescriptions;
        [SerializedDictionary("Item ID", "Hints")] public SerializedDictionary<string, List<int>> Schemas;

        public Dictionary<string, string> MechParts
        {
            get => _mechParts;
        }

        public void SetDefaults()
        {
            _mechParts["Foot"] = "FootLight";
            _mechParts["LeftArm"] = "GadgetHandClaw_V2";
            _mechParts["RightArm"] = "ShortgunV1";
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
#if UNITY_EDITOR

            EditorUtility.SetDirty(this);
#endif

        }

        public void SetMechPartsDict(Dictionary<string, string> newParts)
        {
            _mechParts = newParts;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void SubtractResources(List<string> names)
        {
            foreach (var name in names)
            {
                var existing = resources.Find(
                    (MinerState.StoredResource item) => !string.IsNullOrEmpty(name) && item.name == name);

                if (existing != null)
                {
                    existing.count--;
                }

                var inInventory = InventoryItems.Find((ResourceDto itemDto) => !string.IsNullOrEmpty(name) && itemDto.name == name);

                if (inInventory != null)
                {
                    inInventory.count--;
                    if (inInventory.count <= 0)
                    {
                        inInventory.name = null;
                    }
                }
            }

            resources.RemoveAll((MinerState.StoredResource item) => item.count == 0);
        }
        public ResourceDto FindFreeInventorySlot()
        {
            ResourceDto found = null;

            for (int i = 0; i < InventoryItems.Count; i++)
            {
                var res = InventoryItems[i];

                if (string.IsNullOrEmpty(res.name))
                {
                    found = res;

                    break;
                }
            }

            return found;
        }

        public List<int> FindSchema(string id) {
            return Schemas.ContainsKey(id) ? Schemas[id] : null;
        }

        public RecipieItem FindRecipe(string id) {
            return Recipies.Find((RecipieItem item) => item.Id == id);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(MainStorage))]
    public class MainStorageEditor : Editor
    {
        MainStorage comp;

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
#endif
}