using Unity.Entities;
using UnityEngine;

public class GroundAuthoring : MonoBehaviour
{
    public class GroundAuthoringBaker : Baker<GroundAuthoring>
    {
        public override void Bake(GroundAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<GroundComponent>(entity);
        }
    }
}


