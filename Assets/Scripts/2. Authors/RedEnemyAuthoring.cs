using Unity.Entities;
using UnityEngine;

class RedEnemyAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    class EnemyAuthoringBaker : Baker<RedEnemyAuthoring>
    {
        public override void Bake(RedEnemyAuthoring authoring)
        {
            Entity enemyEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(enemyEntity, new RedEnemyComponents
            {
                MoveSpeed = authoring.MoveSpeed
            });
        }
    }
}