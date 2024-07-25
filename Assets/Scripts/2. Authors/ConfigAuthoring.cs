using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ConfigAuthoring : MonoBehaviour
{
    public float Size;
    public GameObject GroundPrefab;
    public GameObject PlayerPrefab;
    public GameObject RedEnemyPrefab;
    public float RedEnemySpawnRate;
    public float RedEnemySpawnFrequency;

    public class ConfigAuthoringBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ConfigComponents
            {
                Size = authoring.Size,
                GroundPrefab = GetEntity(authoring.GroundPrefab, TransformUsageFlags.Dynamic),
                PlayerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic)
            });

            AddComponent(entity, new RedEnemyPrefabComponent
            {
                RedEnemyPrefab = GetEntity(authoring.RedEnemyPrefab, TransformUsageFlags.Dynamic),
                RedEnemySpawnPosition = authoring.transform.position,
                RedEnemyNextSpawnTime = 0.0f,
                RedEnemySpawnRate = authoring.RedEnemySpawnRate,
                RedEnemySpawnFrequency = authoring.RedEnemySpawnFrequency,
            });
        }
    }
}