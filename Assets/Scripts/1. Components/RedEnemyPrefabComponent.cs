using Unity.Entities;
using Unity.Mathematics;

public struct RedEnemyPrefabComponent : IComponentData
{
    public Entity RedEnemyPrefab;
    public float3 RedEnemySpawnPosition;
    public float RedEnemyNextSpawnTime;
    public float RedEnemySpawnRate;
    public float RedEnemySpawnFrequency;
}
