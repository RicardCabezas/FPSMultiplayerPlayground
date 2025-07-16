using Unity.Entities;
using UnityEngine;

namespace MultiplayerAdditions.Buffs
{
    public class BuffRequestAuthoring : MonoBehaviour
    {
        class Baker : Baker<BuffRequestAuthoring>
        {
            public override void Bake(BuffRequestAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<BuffRequest>(entity);
            }
        }
    }
}