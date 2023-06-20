using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TestWorldSystem : ISystem
{
    // public HexCellAspect.Lookup HexCellAspectFromEntity;
    // public HexCellAspect.TypeHandle HexCellAspectAll;
    public EntityQuery query;

    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HexCellNeighborsEntity>();
        // HexCellAspectFromEntity = new HexCellAspect.Lookup(ref state);
        // HexCellAspectAll = new HexCellAspect.TypeHandle(ref state);
        // query = new EntityQueryBuilder(Allocator.Temp)
        // .WithAll<HexCoordinates>()
        // .Build(ref state);
    }
    [BurstCompile]
    void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        EntityCommandBuffer ECB = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        // HexCellAspectFromEntity.Update(ref state);
        var jobHandle = new TestHexCellJob
        {
            ECB = ECB,
            // HexCellAspectFromEntity = HexCellAspectFromEntity,
            // HexCellAspectAll = HexCellAspectAll,
            // Query = query,
            // hexCellsEntitys = query.ToEntityArray(Allocator.TempJob),
            // hexCoordinates = query.ToComponentDataArray<HexCoordinates>(Allocator.TempJob),
        };
        jobHandle.Schedule();
    }

    [BurstCompile]
    public partial struct TestHexCellJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        // public HexCellAspect.Lookup HexCellAspectFromEntity;
        // public HexCellAspect.TypeHandle HexCellAspectAll;
        // public EntityQuery Query;
        // public NativeArray<Entity> hexCellsEntitys;
        // public NativeArray<HexCoordinates> hexCoordinates;

        [BurstCompile]
        public void Execute(HexCellAspect hexCell)
        {
            // Debug.Log(hexCell.GetNeighbor(0));
            // Debug.Log(string.Format("Entity: {0} neighbour:{1}", hexCell.ToStringCoordinates(), HexCellAspectFromEntity[hexCell.GetNeighbor(HexDirection.E)].ToStringCoordinates()));
            // ECB.SetComponentEnabled<HexCellRefresh>(hexCell.Entity,true);

            // geten
            // HexCellAspectFromEntity.
            // HexCellAspectAll.Resolve()
            // foreach (var item in hexCellsEntitys)
            // {
            //     Debug.Log(item);
            // }
            // foreach (var item in hexCoordinates)
            // {
            //     Debug.Log(item);
            // }
            // Debug.Log(hexCell.entity);
            // Debug.Log(hexCell.GetNeighborCoordinate(0).ToString());
            // Debug.Log(HexCellAspectFromEntity[hexCell.GetNeighbor(HexDirection.NE)]);
        }
    }
}