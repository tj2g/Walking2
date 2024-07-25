using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial struct RedEnemySpawnSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponents>();
        state.RequireForUpdate<RedEnemySpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

        var config = SystemAPI.GetSingleton<ConfigComponents>();
        float3 playerLocation = new float3(0f, 0f, 0f);
        var random = RandomGenerator.GetRandomGenerator();
        var elapsedTime = SystemAPI.Time.ElapsedTime;

        foreach (var (transform, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerComponent>>())
        {
            playerLocation = transform.ValueRO.Position;
        }

        new ProcessRedEnemySpawnJob
        {
            ElapsedTime = elapsedTime,
            Ecb = ecb,
            GroundSize = config.Size * config.Size,
            PlayerPosition = playerLocation,
            Rand = random
        }.ScheduleParallel();
    }

    [BurstCompile]
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }

    [BurstCompile]
    public partial struct ProcessRedEnemySpawnJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public double ElapsedTime;
        public float GroundSize;
        public float3 PlayerPosition;
        public Random Rand;

        private void Execute([ChunkIndexInQuery] int chunkIndex, ref RedEnemyPrefabComponent spawner)
        {
            if (spawner.RedEnemyNextSpawnTime < ElapsedTime)
            {
                var RandomRedEnemyPos = new float3(0f, 0f, 0f);

                var count = 0;

                while (count <= spawner.RedEnemySpawnFrequency)
                {
                    Entity entity = Ecb.Instantiate(chunkIndex, spawner.RedEnemyPrefab);
                    do
                    {
                        RandomRedEnemyPos = new float3(Rand.NextFloat(-GroundSize, GroundSize), 1, Rand.NextFloat(-GroundSize, GroundSize));
                    } while (math.distance(PlayerPosition, RandomRedEnemyPos) <= 5f);

                    Ecb.SetComponent(chunkIndex, entity, LocalTransform.FromPositionRotation(RandomRedEnemyPos, quaternion.Euler(0f, RotationHelper.GetLookDirection(RandomRedEnemyPos, PlayerPosition), 0f))); // Sets both the position and the rotation of the enemy
                    count++;
                }
                spawner.RedEnemyNextSpawnTime = (float)ElapsedTime + spawner.RedEnemySpawnRate;
            }
        }
    }
}


