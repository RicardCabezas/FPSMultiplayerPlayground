using Unity.Entities;
using Unity.NetCode;

namespace Unity.Template.CompetitiveActionMultiplayer.Implementation
{
    [GhostComponent]
    public struct FirstPersonCharacterSocketHolderComponent : IComponentData
    {
        public Entity HealthBarSocketEntity;
    }
}