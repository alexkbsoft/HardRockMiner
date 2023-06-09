using System;
using System.Collections.Generic;

namespace DataLayer
{
    [Serializable]
    public class GameDto
    {
        public List<BlockDto> Blocks = new();
    }
    
    [Serializable]
    public class BlockDto
    {
        public string Type;
        public float Life;
        public float X;
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
    }
}