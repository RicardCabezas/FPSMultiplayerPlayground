using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Template.CompetitiveActionMultiplayer;
using Unity.Transforms;

namespace MultiplayerAdditions.PredictedFeedback
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProjectileHitFeedbackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameResources>();
            state.RequireForUpdate<GhostCollection>();
            state.RequireForUpdate<NetworkId>(); // Needed to assign GhostOwner
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var gameResources = SystemAPI.GetSingleton<GameResources>();

            foreach (var (projectile, owner, entity) in SystemAPI
                         .Query<RefRO<PrefabProjectile>, RefRO<GhostOwner>>()
                         .WithNone<DamageHitFeedback>()
                         .WithEntityAccess())
            {
                if (projectile.ValueRO.HasHit == 1 && projectile.ValueRO.HitEntity != Entity.Null)
                {
                    ecb.AddComponent<DamageHitFeedback>(entity);

                    float3 hitPos = float3.zero;
                    if (SystemAPI.HasComponent<LocalToWorld>(projectile.ValueRO.HitEntity))
                    {
                        hitPos = SystemAPI.GetComponent<LocalToWorld>(projectile.ValueRO.HitEntity).Position;
                    }

                    Entity hitFeedbackGhost = ecb.Instantiate(gameResources.OnHitFeedbackGhost);

                    ecb.SetComponent(hitFeedbackGhost, new HitFeedbackEvent
                    {
                        Position = hitPos
                    });

                    ecb.SetComponent(hitFeedbackGhost, new GhostOwner
                    {
                        NetworkId = owner.ValueRO.NetworkId
                    });

                    ecb.AddComponent(hitFeedbackGhost, new LifeTime()
                    {
                        LifeTimeSeconds = 1f
                    });
                }
            }
        }
    }
}