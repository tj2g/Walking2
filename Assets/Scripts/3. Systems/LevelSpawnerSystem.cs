using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateBefore(typeof(TransformSystemGroup))]
partial struct LevelSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponents>();
        state.RequireForUpdate<LevelSpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<ConfigComponents>();

        var ground = state.EntityManager.Instantiate(config.GroundPrefab);

        state.EntityManager.SetComponentData(ground, new LocalTransform 
        { 
            Scale = config.Size,
        });
    }
}
