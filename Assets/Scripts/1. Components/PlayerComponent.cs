using Unity.Entities;
using Unity.Mathematics;

public struct PlayerComponent : IComponentData
{
    public float MoveSpeed;
    public float JumpForce;
    public bool IsJumping;
}