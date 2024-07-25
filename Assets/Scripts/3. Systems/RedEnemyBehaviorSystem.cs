using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;

partial struct RedEnemyBehaviorSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponents>();
        state.RequireForUpdate<RedEnemyBehaviour>();
        state.RequireForUpdate<PlayerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        var player = SystemAPI.GetSingletonEntity<PlayerComponent>();

        var playerLocation = SystemAPI.GetComponent<LocalTransform>(player).Position;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var deltaTime = SystemAPI.Time.DeltaTime;

        //Movement towards the player
        foreach (var (transform, enemy, redEnemy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<RedEnemyComponents>>().WithEntityAccess())
        {
            // Moving the enemy closer to the player as well as turning the enemy to face the character
            transform.ValueRW.Rotation = quaternion.Euler(0f, RotationHelper.GetLookDirection(transform.ValueRO.Position, playerLocation), 0f);
            transform.ValueRW.Position += transform.ValueRO.Forward() * enemy.ValueRO.MoveSpeed * deltaTime;

            /*
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

            //physicsWorldSingleton.CapsuleCastAll(playerLocation, transform.ValueRO.Position, 1f, transform.ValueRO.Forward(), 1f, ref hits, CollisionFilter.Default);

            foreach (ColliderCastHit hit in hits)
            {
                ecb.DestroyEntity(redEnemy);
            }
            //*/

            ///*
            //Checking to see how close the enemy is to the player
            if (math.distance(playerLocation, transform.ValueRO.Position) <= 1f)
            {
                ecb.DestroyEntity(redEnemy);
            }
            //*/
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
