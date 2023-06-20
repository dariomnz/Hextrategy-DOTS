using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial struct HexGridData : IComponentData
{
    public int cellSizeX, cellSizeZ;
    public int chunkSizeX, chunkSizeZ;
    public Entity HexCellPrefab;
    public Entity HexChunkPrefab;
}


// public partial struct HexTerrainColor
// {   
//     public NativeArray<HexTerrainType> TKey;
//     public NativeArray<float4> TValue;

//     public float4 GetValue(HexTerrainType key)
//     {
//         for (int i = 0; i < TKey.Length; i++)
//         {
//             if (TKey[i] == key)
//                 return TValue[i];
//         }
//         return new float4();
//     }
// }
[InternalBufferCapacity(10)]
public partial struct HexTerrainColor : IBufferElementData
{
    public HexTerrainType Key;
    public float4 Value;
}

// public partial struct HexTerrainColor
// {
//     // public BlobArray<HexTerrainType> TKey;
//     public BlobArray<float4> TValue;

//     public float4 GetValue(HexTerrainType key)
//     {
//         // for (int i = 0; i < TKey.Length; i++)
//         // {
//         //     if (TKey[i] == key)
//         //         return TValue[i];
//         // }
//         return new float4();
//     }
// }