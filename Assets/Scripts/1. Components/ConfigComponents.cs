using Unity.Entities;

public struct ConfigComponents : IComponentData
{
    public float Size;
    public Entity GroundPrefab;
    public Entity PlayerPrefab;
}
