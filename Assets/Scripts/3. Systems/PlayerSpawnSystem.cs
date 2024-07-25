using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(LevelSpawnerSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
partial struct PlayerSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponents>();
        state.RequireForUpdate<PlayerSpawn>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<ConfigComponents>();

        var player = state.EntityManager.Instantiate(config.PlayerPrefab);
    }
}
