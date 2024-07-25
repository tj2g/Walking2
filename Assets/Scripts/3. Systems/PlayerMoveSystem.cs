using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

partial struct PlayerMoveSystem : ISystem
{
    private EntityManager entityManager;

    private Entity playerEntity;
    private Entity inputEntity;

    private PlayerComponent playerComponent;
    private InputComponents inputComponents;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerMove>();
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
            physicsVelocity.ValueRW.Linear.x = inputComponents.Movement.x * playerComponent.MoveSpeed;// * SystemAPI.Time.DeltaTime; Time made it incredibly slow, but also added in the floating, when interacting with jumping made it oddly super fast
            physicsVelocity.ValueRW.Linear.z = inputComponents.Movement.y * playerComponent.MoveSpeed;// * SystemAPI.Time.DeltaTime;
        }

        //Calls the to set location and look direction for player.
        //PlayerMove(ref state);
    }
    /*  Did this before I realized there was an easier way to write the code
    [BurstCompile]
    //Updates the character for it's location and where it's looking.
    private void PlayerMove(ref SystemState state)
    {
        ///*
        RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(playerEntity);

        physicsVelocity.ValueRW.Linear += new float3(
            inputComponents.Movement.x * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime,
            0,
            inputComponents.Movement.y * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime);
        // */

        /*
        //Move the player
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        playerTransform.Position.x += inputComponents.Movement.x * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime;
        playerTransform.Position.z += inputComponents.Movement.y * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime;
        
        entityManager.SetComponentData(playerEntity, playerTransform);
        
    }//*/
}
