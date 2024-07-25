using Unity.Entities;
using Unity.Mathematics;

public struct InputComponents : IComponentData
{
    public float2 Movement;
    public float2 LookDirection;
    public float Scrolling;
    public float Jumping;
}
