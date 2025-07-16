using Unity.Entities;

namespace Unity.Template.CompetitiveActionMultiplayer.Implementation
{
    public struct HealthBarProxy : IComponentData
    {
        public Entity PlayerEntity;
    }
}