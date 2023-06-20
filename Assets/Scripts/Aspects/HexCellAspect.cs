using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct HexCellAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<HexCellNeighborsEntity> _HexCellNeighborsEntity;
    private readonly RefRO<HexCoordinates> _HexCoordinates;
    private readonly RefRW<HexCellTerrainData> _HexCellTerrainData;

    public int x => _HexCoordinates.ValueRO.X;
    public int y => _HexCoordinates.ValueRO.Y;
    public int z => _HexCoordinates.ValueRO.Z;

    public HexTerrainType HexTerrainType
    {
        get => _HexCellTerrainData.ValueRO.TerrainType;
        set
        {
            _HexCellTerrainData.ValueRW.TerrainType = value;
        }
    }
    public int Elevation => _HexCellTerrainData.ValueRO.Elevation;

    public string ToStringCoordinates() => _HexCoordinates.ValueRO.ToString();

    // public HexCoordinates GetNeighborCoordinate(HexDirection i) => _HexCellNeighborsCoordinates.ValueRO.GetNeighborCoordinate(i);
    public Entity GetNeighbor(HexDirection i) => _HexCellNeighborsEntity.ValueRO.GetNeighbor(i);
}