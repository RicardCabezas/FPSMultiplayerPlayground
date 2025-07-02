using MultiplayerAdditions.PredictedFeedback;
using Unity.Entities;

public partial struct OnProjectileHitPredictedFeedback
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct OnProjectileHitPredictedFeedbackCleanup : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
                
            float deltaTime = SystemAPI.Time.DeltaTime;
                
            foreach (var (damageHitFeedback, entity) in SystemAPI
                         .Query<RefRW<DamageHitFeedback>>()
                         .WithEntityAccess())
            {
                damageHitFeedback.ValueRW.LifeTime -= deltaTime;

                if (damageHitFeedback.ValueRO.LifeTime <= 0f)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}