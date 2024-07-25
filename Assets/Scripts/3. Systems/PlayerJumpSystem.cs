using Unity.Entities;
using Unity.Burst;
using Unity.Physics;

[UpdateAfter(typeof(PlayerSpawnSystem))]

public partial struct PlayerSystem : ISystem
{
    private EntityManager entityManager;

    private Entity playerEntity;
    private Entity inputEntity;

    private PlayerComponent playerComponent;
    private InputComponents inputComponents;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerJump>();
        state.RequireForUpdate<PlayerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        inputEntity = SystemAPI.GetSingletonEntity<InputComponents>();

        playerComponent = entityManager.GetComponentData<PlayerComponent>(playerEntity);
        inputComponents = entityManager.GetComponentData<InputComponents>(inputEntity);

        foreach (var (physicsVelocity, player) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<PlayerComponent>>())
        {
            if (inputComponents.Jumping == 1)
            {
                physicsVelocity.ValueRW.Linear.y = playerComponent.JumpForce;
            }
        }
    }
}
