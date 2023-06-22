using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using System.Collections.Generic;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct GenerateWorldSystem : ISystem
{
    // public HexCellAspect.Lookup HexCellAspectFromEntity;

    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HexGridData>();
        // HexCellAspectFromEntity = new HexCellAspect.Lookup(ref state);
    }
    [BurstCompile]
    void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        // Entity hexGridEntity = SystemAPI.GetSingletonEntity<HexGridData>();
        // HexGridAspect hexGrid = SystemAPI.GetAspect<HexGridAspect>(hexGridEntity);

        // EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
        EntityCommandBuffer ECB = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        var jobHandle = new SpawnHexCellsJob
        {
            ECB = ECB,
            // hexGridEntity = hexGridEntity,
        };

        jobHandle.Schedule();
    }

    [BurstCompile]
    public partial struct SpawnHexCellsJob : IJobEntity
    {
        public EntityCommandBuffer ECB;

        [BurstCompile]
        void SetNeighbor(HexDirection direction, Entity cell, Entity cellNeighbor, NativeArray<Entity> cellArray, NativeArray<Entity> cellNeighborsArray)
        {
            cellArray[(int)direction] = cellNeighbor;
            cellNeighborsArray[(int)direction.Opposite()] = cell;
        }

        [BurstCompile]
        public LocalTransform GetHexCellSpawnPoint(int x, int z)
        {
            float _x = (x + z * 0.5f) % HexChunkConstData.chunkSizeX * HexCellConstData.xDiameter;
            float _z = z % HexChunkConstData.chunkSizeZ * HexCellConstData.zDiameter;
            return new LocalTransform
            {
                Position = new float3(_x, 0f, _z),
                Rotation = quaternion.identity,
                Scale = 1f
            };
        }

        [BurstCompile]
        public void Execute(HexGridAspect hexGrid)
        {

            NativeArray<Entity> chunks = new NativeArray<Entity>(hexGrid.chunkSize, Allocator.Temp);

            for (int z = 0, i = 0; z < hexGrid.ChunkSizeZ; z++)
            {
                for (int x = 0; x < hexGrid.ChunkSizeX; x++)
                {
                    chunks[i] = ECB.Instantiate(hexGrid.HexChunkPrefab);
                    ECB.AddComponent(chunks[i], new Parent { Value = hexGrid.Entity });

                    float3 newPosition;
                    newPosition.x = HexChunkConstData.chunkSizeX * HexCellConstData.xDiameter * x;
                    newPosition.y = 0;
                    newPosition.z = HexChunkConstData.chunkSizeZ * HexCellConstData.zDiameter * z;
                    ECB.SetComponent(chunks[i], new LocalTransform { Position = newPosition, Rotation = quaternion.identity, Scale = 1 });
                    i++;
                }
            }

            NativeArray<Entity> cells = new NativeArray<Entity>(hexGrid.cellSize, Allocator.Temp);
            NativeArray<NativeArray<Entity>> tempNeighbourArray = new NativeArray<NativeArray<Entity>>(hexGrid.cellSize, Allocator.Temp);
            // Entity[] cells = new Entity[hexGrid.cellSize];
            // Entity[][] tempNeighbourArray = new Entity[hexGrid.cellSize][];

            for (int i = 0; i < hexGrid.cellSize; i++)
            {
                tempNeighbourArray[i] = new NativeArray<Entity>(6, Allocator.Temp); ;
            }


            // var LinkedEntityBuffer = ECB.AddBuffer<LinkedEntityGroup>(hexGrid.Entity);
            // LinkedEntityBuffer.Add(hexGrid.Entity);
            for (int z = 0, i = 0; z < hexGrid.CellSizeZ; z++)
            {
                for (int x = 0; x < hexGrid.CellSizeX; x++)
                {
                    Entity newHexCell = cells[i] = ECB.Instantiate(hexGrid.HexCellPrefab);

                    int chunkX = x / HexChunkConstData.chunkSizeX;
                    int chunkZ = z / HexChunkConstData.chunkSizeZ;
                    Entity chunk = chunks[chunkX + chunkZ * hexGrid.ChunkSizeX];

                    ECB.AddComponent(newHexCell, new Parent { Value = chunk });
                    // LinkedEntityBuffer.Add(new LinkedEntityGroup { Value = newHexCell });
                    var newHexCoordinates = new HexCoordinates(x, z);
                    ECB.SetComponent(newHexCell, newHexCoordinates);
                    var newHexCellTransform = GetHexCellSpawnPoint(x, z);
                    ECB.SetComponent(newHexCell, newHexCellTransform);
                    i++;
                }
            }

            for (int z = 0, i = 0; z < hexGrid.CellSizeZ; z++)
            {
                for (int x = 0; x < hexGrid.CellSizeX; x++)
                {
                    Entity cell = cells[i];

                    if (x > 0)
                    {
                        SetNeighbor(HexDirection.W, cell, cells[i - 1], tempNeighbourArray[i], tempNeighbourArray[i - 1]);
                    }
                    else
                    {
                        SetNeighbor(HexDirection.W, cell, cells[i + hexGrid.CellSizeX - 1], tempNeighbourArray[i], tempNeighbourArray[i + hexGrid.CellSizeX - 1]);
                        if ((z & 1) == 0)
                        {
                            SetNeighbor(HexDirection.NW, cell, cells[i + hexGrid.CellSizeX * 2 - 1], tempNeighbourArray[i], tempNeighbourArray[i + hexGrid.CellSizeX * 2 - 1]);
                            if (z != 0)
                                SetNeighbor(HexDirection.SW, cell, cells[i - 1], tempNeighbourArray[i], tempNeighbourArray[i - 1]);
                        }
                    }
                    if (z > 0)
                    {
                        if ((z & 1) == 0)
                        {
                            SetNeighbor(HexDirection.SE, cell, cells[i - hexGrid.CellSizeX], tempNeighbourArray[i], tempNeighbourArray[i - hexGrid.CellSizeX]);
                            if (x > 0)
                                SetNeighbor(HexDirection.SW, cell, cells[i - hexGrid.CellSizeX - 1], tempNeighbourArray[i], tempNeighbourArray[i - hexGrid.CellSizeX - 1]);
                        }
                        else
                        {
                            SetNeighbor(HexDirection.SW, cell, cells[i - hexGrid.CellSizeX], tempNeighbourArray[i], tempNeighbourArray[i - hexGrid.CellSizeX]);
                            if (x < hexGrid.CellSizeX - 1)
                                SetNeighbor(HexDirection.SE, cell, cells[i - hexGrid.CellSizeX + 1], tempNeighbourArray[i], tempNeighbourArray[i - hexGrid.CellSizeX + 1]);
                        }
                    }
                    else
                    {
                        SetNeighbor(HexDirection.SE, cell, cells[cells.Length - (hexGrid.CellSizeX - i)], tempNeighbourArray[i], tempNeighbourArray[cells.Length - (hexGrid.CellSizeX - i)]);
                        if (x == 0)
                            SetNeighbor(HexDirection.SW, cell, cells[cells.Length - 1], tempNeighbourArray[i], tempNeighbourArray[cells.Length - 1]);
                        else
                            SetNeighbor(HexDirection.SW, cell, cells[cells.Length - (hexGrid.CellSizeX - i + 1)], tempNeighbourArray[i], tempNeighbourArray[cells.Length - (hexGrid.CellSizeX - i + 1)]);
                    }

                    i++;
                }
            }



            for (int i = 0; i < hexGrid.cellSize; i++)
            {
                ECB.SetComponent(cells[i], new HexCellNeighborsEntity(tempNeighbourArray[i]));
            }

        }
    }

    [BurstCompile]
    public partial struct CreateTerrainJob : IJobEntity
    {
        public EntityCommandBuffer ECB;

        [BurstCompile]
        public void Execute(HexGridAspect hexGrid)
        {

        }
    }
}
