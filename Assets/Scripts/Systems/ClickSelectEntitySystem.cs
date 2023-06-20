using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Rendering;
using RaycastHit = Unity.Physics.RaycastHit;

public partial class ClickSelectEntitySystem : SystemBase
{
    Camera _mainCamera;
    HexCellAspect.Lookup HexCellAspectLookUp;

    protected override void OnCreate()
    {
        _mainCamera = Camera.main;
        HexCellAspectLookUp = new HexCellAspect.Lookup();
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var rayStart = ray.origin;
            var rayEnd = ray.GetPoint(1000);
            SelectEntity(rayStart, rayEnd, out RaycastHit raycastHit);
            Debug.Log(raycastHit.Entity);

            EntityCommandBuffer ECB = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            Entity hexGridEntity = SystemAPI.GetSingletonEntity<HexGridData>();
            HexGridAspect hexGrid = SystemAPI.GetAspect<HexGridAspect>(hexGridEntity);
            // var buffer = EntityManager.GetBuffer<HexTerrainColor>(hexGridEntity, true);
            // HexGridAspect.GetHexTerrainColor(buffer, HexTerrainType.Sand, out float4 outColor);
            // Debug.Log(outColor);

            // for (int i = 0; i < hexGrid.array.Length; i++)
            // {
            //     Debug.Log($"Value: {hexGrid.array[i].Value}");
            //     // Debug.Log($"Key: {hexGrid.HexTerrainColor.TKey[i]} value: {hexGrid.HexTerrainColor.TValue[i]}");
            // }
            var lookup = GetComponentLookup<HexCellTerrainData>();
            lookup.GetRefRW(raycastHit.Entity).ValueRW.TerrainType = HexTerrainType.Sand;


            var lookupRefresh = GetComponentLookup<HexCellRefresh>();
            lookupRefresh.SetComponentEnabled(raycastHit.Entity, true);
            // lookup.TryGetComponent
            // if (SystemAPI.HasComponent<MaterialColor>(raycastHit.Entity))

            //     ECB.SetComponent<MaterialColor>(raycastHit.Entity, new MaterialColor { Value = hexGrid.GetHexTerrainColor(EntityManager, HexTerrainType.Sand) });
            // else
            //     ECB.AddComponent<MaterialColor>(raycastHit.Entity, new MaterialColor { Value = hexGrid.GetHexTerrainColor(EntityManager, HexTerrainType.Sand) });
        }
    }

    private void SelectEntity(float3 rayStart, float3 rayEnd, out RaycastHit raycastHit)
    {
        var rayCastInput = new RaycastInput
        {
            Start = rayStart,
            End = rayEnd,
            Filter = CollisionFilter.Default,
        };
        SystemAPI.GetSingleton<PhysicsWorldSingleton>().CastRay(rayCastInput, out raycastHit);
    }
}