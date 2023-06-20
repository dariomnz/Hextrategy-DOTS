using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using System.Collections.Generic;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct RefreshHexCellSystem : ISystem
{
    // public HexCellAspect.Lookup HexCellAspectFromEntity;

    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HexCellRefresh>();
    }

    [BurstCompile]
    void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        // state.Enabled = false;
        Entity hexGridEntity = SystemAPI.GetSingletonEntity<HexGridData>();
        HexGridAspect hexGrid = SystemAPI.GetAspect<HexGridAspect>(hexGridEntity);
        DynamicBuffer<HexTerrainColor> HexTerrainColorBuffer = SystemAPI.GetBuffer<HexTerrainColor>(hexGridEntity);


        // EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
        EntityCommandBuffer.ParallelWriter ECB = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        var aux = state.GetComponentLookup<MaterialColor>(true);

        var jobHandle = new HexCellRefreshJob
        {
            ECB = ECB,
            MaterialColorLookUp = aux,
            // EntityManager = state.EntityManager,
            // hexGrid = hexGrid,
            HexTerrainColorBuffer = HexTerrainColorBuffer,
        };

        jobHandle.ScheduleParallel();
    }


    [BurstCompile]
    public partial struct HexCellRefreshJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ComponentLookup<MaterialColor> MaterialColorLookUp;
        // [ReadOnly] public EntityManager EntityManager;
        // public HexGridAspect hexGrid;
        [ReadOnly] public DynamicBuffer<HexTerrainColor> HexTerrainColorBuffer;
        // public HexCellAspect.Lookup HexCellAspectFromEntity;
        // public HexCellAspect.TypeHandle HexCellAspectAll;
        // public EntityQuery Query;
        // public NativeArray<Entity> hexCellsEntitys;
        // public NativeArray<HexCoordinates> hexCoordinates;

        [BurstCompile]
        public void Execute(Entity entity, HexCellAspect hexCell, HexCellRefresh hexCellRefresh, [ChunkIndexInQuery] int sortKey)
        {
            ECB.SetComponentEnabled<HexCellRefresh>(sortKey, entity, false);
            // Debug.Log($"HasComponent {MaterialColorLookUp.HasComponent(entity)}");
            // ref DynamicBuffer<HexTerrainColor> aux = ref HexTerrainColorBuffer;
            HexGridAspect.GetHexTerrainColor(ref HexTerrainColorBuffer, hexCell.HexTerrainType, out float4 outColor);

            if (MaterialColorLookUp.HasComponent(entity))
                ECB.SetComponent<MaterialColor>(sortKey, entity, new MaterialColor { Value = outColor });
            else
                ECB.AddComponent<MaterialColor>(sortKey, entity, new MaterialColor { Value = outColor });
            Debug.Log($"Refreshing Entity: {entity} outColor:{outColor} sortKey:{sortKey}");
        }
    }
}