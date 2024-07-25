using Unity.Entities;
using UnityEngine;

public class SystemExecuteAuthoring : MonoBehaviour
{
    [Header("Spinup Spawner Systems")]
    public bool LevelSpawner;
    public bool PlayerSpawn;

    [Header("Camera Systems")]
    public bool PlayerCamera;

    [Header("Player Systems")]
    public bool PlayerMove;
    public bool PlayerLook;
    public bool PlayerJump;

    [Header("Enemy Systems")]
    public bool RedEnemySpawner;
    public bool RedEnemyBehaviour;

    public class SystemExecuteAuthoringBaker : Baker<SystemExecuteAuthoring>
    {
        public override void Bake(SystemExecuteAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            if (authoring.LevelSpawner) AddComponent<LevelSpawner>(entity);
            if (authoring.PlayerSpawn) AddComponent<PlayerSpawn>(entity);
            if (authoring.PlayerCamera) AddComponent<PlayerCamera>(entity);
            if (authoring.PlayerMove) AddComponent<PlayerMove>(entity);
            if (authoring.PlayerLook) AddComponent<PlayerLook>(entity);
            if (authoring.PlayerJump) AddComponent<PlayerJump>(entity);
            if (authoring.RedEnemySpawner) AddComponent<RedEnemySpawner>(entity);
            if (authoring.RedEnemyBehaviour) AddComponent<RedEnemyBehaviour>(entity);
        }
    }
}