using System;
using System.Collections.Generic;

namespace DataLayer
{
    [Serializable]
    public class GameDto
    {
        public List<BlockDto> Blocks = new();
        public List<SegmentDto> Walls = new();
        public List<SegmentDto> Floors = new();
        public List<SegmentDto> Decorations = new();
        public List<SegmentDto> Spawners = new();
        public List<SegmentDto> Pillars = new();
    }

    [Serializable]
    public class BlockDto
    {
        public string Type;
        public float Life;
        public float X;
        public float Y;
        public float Z;

    }

    [Serializable]
    public class SegmentDto
    {
        public string Type;
        public float YRotation;
        public float X;
        public float Y;
        public float Z;

    }

    [Serializable]
    public class ResourceDto
    {
        public string name;
        public int count;
    }

    [Serializable]
    public class StorageDto
    {
        public List<ResourceDto> Resources = new();
        public List<ResourceDto> Inventory = new();
        public List<MechPartDto> MechParts = new();
    }

    [Serializable]
    public class MechPartDto
    {
        public string name;
        public string item;
    }

    [Serializable]
    public class ItemDescriptionDto
    {
        public string id;
        public string title;
        public string description;
    }

    [Serializable]
    public class ItemDescription
    {
        public string title;
        public string description;
    }


    [Serializable]
    public class ItemsInfoDto
    {
        public List<ItemDescriptionDto> Descriptions = new();

    }
}