using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public readonly partial struct HexGridAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<HexGridData> _HexGridData;

    public int CellSizeX { get => _HexGridData.ValueRO.cellSizeX; }
    public int CellSizeZ { get => _HexGridData.ValueRO.cellSizeZ; }

    public int ChunkSizeX { get => _HexGridData.ValueRO.chunkSizeX; }
    public int ChunkSizeZ { get => _HexGridData.ValueRO.chunkSizeZ; }

    public int cellSize => _HexGridData.ValueRO.cellSizeX * _HexGridData.ValueRO.cellSizeZ;
    public int chunkSize => _HexGridData.ValueRO.chunkSizeX * _HexGridData.ValueRO.chunkSizeZ;

    public Entity HexCellPrefab => _HexGridData.ValueRO.HexCellPrefab;
    public Entity HexChunkPrefab => _HexGridData.ValueRO.HexChunkPrefab;

    [BurstCompile]
    public static void GetHexTerrainColor(ref DynamicBuffer<HexTerrainColor> buffer, HexTerrainType type, out float4 outColor)
    {
        foreach (var item in buffer)
        {
            if (item.Key == type)
            {
                outColor = item.Value;
                return;
            }
        }
        outColor = new float4();
    }
}