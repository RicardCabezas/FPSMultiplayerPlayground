using Unity.Entities;

namespace Unity.Template.CompetitiveActionMultiplayer.Implementation
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct AttachPlayerVisualsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .ValueRW.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (character, entity) in SystemAPI
                         .Query<FirstPersonCharacterSocketHolderComponent>()
                         .WithNone<HealthBarProxy>()
                         .WithEntityAccess())
            {
                ecb.AddComponent(character.HealthBarSocketEntity, new HealthBarProxy
                {
                    PlayerEntity = entity
                });
            }
        }
    }
}