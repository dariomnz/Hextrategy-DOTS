using Unity.Collections;
using Unity.Entities;
using Unity.Properties;


public partial struct HexChunkTag : IComponentData { }

public struct HexChunkConstData
{
    public const int chunkSizeX = 6, chunkSizeZ = 6;
}