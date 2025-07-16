using Unity.Entities;
using UnityEngine;

namespace Unity.Template.CompetitiveActionMultiplayer.Implementation
{
    [DisallowMultipleComponent]
    public class FirstPersonCharacterSocketAuthoring : MonoBehaviour
    {
        public GameObject HealthBarSocket;
        
        public class Baker : Baker<FirstPersonCharacterSocketAuthoring>
        {
            public override void Bake(FirstPersonCharacterSocketAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace);

                AddComponent(entity, new FirstPersonCharacterSocketHolderComponent
                {
                    HealthBarSocketEntity = GetEntity(authoring.HealthBarSocket, TransformUsageFlags.Renderable),
                });
            }
        }
    }
}