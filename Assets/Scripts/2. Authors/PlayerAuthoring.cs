using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerAuthoring : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float CameraOffset;
    public float JumpForce;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(playerEntity, new PlayerComponent
            {
                MoveSpeed = authoring.MoveSpeed,
                JumpForce = authoring.JumpForce,
            });

            AddComponent(playerEntity, new CameraOffsetComponent() 
            {
                CameraOffset = authoring.CameraOffset
            });
        }
    }
}
