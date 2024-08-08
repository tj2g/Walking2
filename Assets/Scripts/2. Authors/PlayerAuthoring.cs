using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerAuthoring : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float CameraOffset;
    public float JumpForce;
    public bool IsJumping;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(playerEntity, new PlayerComponent
            {
                MoveSpeed = authoring.MoveSpeed,
                JumpForce = authoring.JumpForce,
                IsJumping = authoring.IsJumping,
            });

            AddComponent(playerEntity, new CameraOffsetComponent() 
            {
                CameraOffset = authoring.CameraOffset
            });
        }
    }
}
