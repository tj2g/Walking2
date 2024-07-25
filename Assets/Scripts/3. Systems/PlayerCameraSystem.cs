using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PlayerSpawnSystem))]
public partial struct PlayerCameraSystem : ISystem
{
    private EntityManager entityManager;

    private Entity player;
    private Entity input;

    private CameraOffsetComponent cameraOffsetComponent;
    private InputComponents inputComponents;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CameraOffsetComponent>();

        state.RequireForUpdate<PlayerCamera>();
    }

    public void OnUpdate(ref SystemState state)
    { 
        entityManager = state.EntityManager;

        player = SystemAPI.GetSingletonEntity<PlayerComponent>();
        input = SystemAPI.GetSingletonEntity<InputComponents>();

        cameraOffsetComponent = entityManager.GetComponentData<CameraOffsetComponent>(player);
        inputComponents = entityManager.GetComponentData<InputComponents>(input);

        var camera = CameraMono.Camera.transform;

        float3 targetPosition = Vector3.zero;

        //Calls to set the distance from player
        SetNewCameraOffset(ref state);
        camera.rotation = Quaternion.Euler(new float3(90f, 0f, 0f));
        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerComponent>())
        {
            targetPosition = transform.ValueRO.Position + new float3(0f, cameraOffsetComponent.CameraOffset, 0f);
            //camera.rotation = Quaternion.Euler (new float3(90f, 0f, 0f));
            camera.transform.position = targetPosition;
        }
    }

    //Sets the camera distance from the player with a min range of 3 and max of 20, can be changed later
    public void SetNewCameraOffset(ref SystemState state)
    {
        cameraOffsetComponent.CameraOffset -= inputComponents.Scrolling;

        if (cameraOffsetComponent.CameraOffset < 3f)
        {
            cameraOffsetComponent.CameraOffset = 3f;
        }

        if (cameraOffsetComponent.CameraOffset > 20f)
        {
            cameraOffsetComponent.CameraOffset = 20f;
        }

        entityManager.SetComponentData<CameraOffsetComponent>(player, cameraOffsetComponent);
    }
}
