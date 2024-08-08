using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

partial struct PlayerLookSystem : ISystem
{
    private EntityManager entityManager;

    private Entity playerEntity;
    private Entity inputEntity;

    private PlayerComponent playerComponent;
    private InputComponents inputComponents;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerLook>();
        state.RequireForUpdate<PlayerComponent>();
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        inputEntity = SystemAPI.GetSingletonEntity<InputComponents>();

        playerComponent = entityManager.GetComponentData<PlayerComponent>(playerEntity);
        inputComponents = entityManager.GetComponentData<InputComponents>(inputEntity);

        PlayerLook(ref state);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    private void PlayerLook(ref SystemState state)
    {
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        //Keeps the look direction at the last place it was put to.
        Vector2 lookDirection = inputComponents.LookDirection;
        if (lookDirection.sqrMagnitude == 0) { return; }

        Vector2 direction;

        if ((lookDirection.x >= -1 && lookDirection.x <= 1) && (lookDirection.y >= -1 && lookDirection.y <= 1))
        {
            //This is in case the Controller is being used.
            direction = (Vector2)inputComponents.LookDirection;
        }
        else
        {
            //This is in case the mouse is being used.
            direction = (Vector2)inputComponents.LookDirection - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);
        }

        //look towards mouse
        float angle = math.degrees(math.atan2(direction.x, direction.y));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.up);

        entityManager.SetComponentData(playerEntity, playerTransform);
    }
}
