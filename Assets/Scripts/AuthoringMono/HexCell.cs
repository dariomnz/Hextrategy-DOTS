using RotaryHeart.Lib.SerializableDictionary;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public GameObject Renderer;
    public GameObject Collider;
    public GameObject Data;
}

public class HexCellBaker : Baker<HexCell>
{
    public override void Bake(HexCell authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        Entity dataEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic, false, "Data");

        AddComponent<HexCellEntities>(entity, new HexCellEntities
        {
            Renderer = GetEntity(authoring.Renderer, TransformUsageFlags.Renderable),
            Collider = GetEntity(authoring.Collider, TransformUsageFlags.Dynamic),
            Data = dataEntity,
        });

        AddComponent<HexCellNeighborsEntity>(entity);
        AddComponent<HexCoordinates>(entity);
        AddComponent<HexCellRefresh>(entity);

        AddComponent<Parent>(dataEntity, new Parent { Value = entity });
        AddComponent<HexCellTerrainData>(dataEntity, new HexCellTerrainData
        {
            TerrainType = HexTerrainType.Water,
            Elevation = 0
        });
    }
}
