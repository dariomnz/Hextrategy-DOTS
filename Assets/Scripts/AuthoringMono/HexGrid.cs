using RotaryHeart.Lib.SerializableDictionary;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int chunkSizeX = 2, chunkSizeZ = 2;
    public GameObject HexCellPrefab;
    public GameObject HexChunkPrefab;
    public HexTerrainScriptableObject hexTerrainScriptableObject;
}

public class HexGridBaker : Baker<HexGrid>
{
    public override void Bake(HexGrid authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);


        AddComponent(entity, new HexGridData
        {
            cellSizeX = authoring.chunkSizeX * HexChunkConstData.chunkSizeX,
            cellSizeZ = authoring.chunkSizeZ * HexChunkConstData.chunkSizeZ,
            chunkSizeX = authoring.chunkSizeX,
            chunkSizeZ = authoring.chunkSizeZ,
            HexCellPrefab = GetEntity(authoring.HexCellPrefab, TransformUsageFlags.Dynamic),
            HexChunkPrefab = GetEntity(authoring.HexChunkPrefab, TransformUsageFlags.Dynamic),
        });
        var buffer = AddBuffer<HexTerrainColor>(entity);
        var dict = authoring.hexTerrainScriptableObject.TerrainColors;
        foreach (HexTerrainType key in dict.Keys)
        {
            Color color = dict[key];
            buffer.Add(new HexTerrainColor
            {
                Key = key,
                Value = new float4(color.r, color.g, color.b, color.a),
            });
        }
    }

    // public static BlobAssetReference<BlobArray<KeyPairsHexTerrainColor>> CreateClipBlobReference(SerializableDictionaryBase<HexTerrainType, Color> _HexTerrainColors)
    // {

    //     using (var builder = new BlobBuilder(Allocator.Temp))
    //     {
    //         ref var root = ref builder.ConstructRoot<BlobArray<KeyPairsHexTerrainColor>>();
    //         var clips = builder.Allocate(ref root, _HexTerrainColors.Count);
    //         // for (var i = 0; i < _HexTerrainColors.Count; ++i)
    //         // {
    //         //     clips[i].Key = _HexTerrainColors[i].Keys.[i];
    //         //     clips[i].Value = sourceData[i];
    //         // }
    //         int i = 0;
    //         foreach (HexTerrainType key in _HexTerrainColors.Keys)
    //         {
    //             clips[i].Key = key;
    //             Color color = _HexTerrainColors[key];
    //             clips[i].Value = new float4(color.r, color.g, color.b, color.a);
    //             i++;
    //         }
    //         return builder.CreateBlobAssetReference<BlobArray<KeyPairsHexTerrainColor>>(Allocator.Persistent);
    //     }
    // }

    // BlobAssetReference<HexTerrainColor> CreateHexTerrainColors(SerializableDictionaryBase<HexTerrainType, Color> _HexTerrainColors)
    // {
    //     int Count = _HexTerrainColors.Count;
    //     // Create a new builder that will use temporary memory to construct the blob asset
    //     var builder = new BlobBuilder(Allocator.Temp);

    //     // Construct the root object for the blob asset. Notice the use of `ref`.
    //     ref HexTerrainColor hexTerrainColor = ref builder.ConstructRoot<HexTerrainColor>();

    //     // Now fill the constructed root with the data:
    //     // Apples compare to Oranges in the universally accepted ratio of 2 : 1 .
    //     // BlobBuilderArray<HexTerrainType> arrayBuilderTKey = builder.Allocate(ref hexTerrainColor.TKey, Count);
    //     // BlobBuilderArray<float4> arrayBuilderTValue = builder.Allocate(ref hexTerrainColor.TValue, Count);

    //     // int i = 0;
    //     // foreach (HexTerrainType key in _HexTerrainColors.Keys)
    //     // {
    //     //     // arrayBuilderTKey[i] = key;
    //     //     Color color = _HexTerrainColors[key];
    //     //     arrayBuilderTValue[i] = new float4(color.r, color.g, color.b, color.a);
    //     //     i++;
    //     // }

    //     NativeArray<HexTerrainType> TKey = new NativeArray<HexTerrainType>(Count, Allocator.Persistent);
    //     NativeArray<float4> TValue = new NativeArray<float4>(Count, Allocator.Persistent);

    //     int i = 0;
    //     foreach (HexTerrainType key in _HexTerrainColors.Keys)
    //     {
    //         TKey[i] = key;
    //         Color color = _HexTerrainColors[key];
    //         TValue[i] = new float4(color.r, color.g, color.b, color.a);
    //         i++;
    //     }

    //     hexTerrainColor.TKey = TKey;
    //     hexTerrainColor.TValue = TValue;

    //     // Now copy the data from the builder into its final place, which will
    //     // use the persistent allocator
    //     var result = builder.CreateBlobAssetReference<HexTerrainColor>(Allocator.Persistent);

    //     // Make sure to dispose the builder itself so all internal memory is disposed.
    //     builder.Dispose();
    //     return result;
    // }
}
