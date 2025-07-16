using MultiplayerAdditions.PredictedFeedback;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Template.CompetitiveActionMultiplayer;
using Unity.Transforms;

namespace MultiplayerAdditions.PredictedFeedback
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]

    public partial struct OnProjectileHitPredictedFeedback : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagedResources>(); // <-- Ensure prefab is available
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            if (!SystemAPI.TryGetSingleton(out GameResources gameResources))
            {
                return;
            }

            foreach (var (projectile, entity) in SystemAPI
                         .Query<RefRO<PrefabProjectile>>()
                         .WithNone<DamageHitFeedback>()
                         .WithEntityAccess())
            {
                if (projectile.ValueRO.HasHit == 1 && projectile.ValueRO.HitEntity != Entity.Null)
                {
                    Entity fxEntity = ecb.Instantiate(gameResources.OnHitFeedback);
                    ecb.AddComponent<DamageHitFeedback>(entity);
                    ecb.AddComponent(fxEntity, new LifeTime {
                        LifeTimeSeconds = 1f
                    });
                    //Since the Hit entity doesnt have the position of the hit I need to do this workaround
                    if (SystemAPI.HasComponent<LocalToWorld>(projectile.ValueRO.HitEntity))
                    {
                        float3 hitPos = SystemAPI.GetComponent<LocalToWorld>(projectile.ValueRO.HitEntity).Position;

                        ecb.SetComponent(fxEntity, LocalTransform.FromPositionRotation(
                            hitPos, quaternion.identity));
                    }
                }
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct RemoteHitFeedbackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagedResources>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            if (!SystemAPI.TryGetSingleton(out GameResources gameResources))
            {
                return;
            }

            foreach (var (hitEvent, entity) in SystemAPI
                         .Query<RefRO<HitFeedbackEvent>>()
                         .WithNone<HitFeedbackEventProcessed>()
                         .WithEntityAccess())
            {
                // Check if the entity is owned by the local player to don't draw it again
                if (SystemAPI.HasComponent<GhostOwner>(entity))
                {
                    var owner = SystemAPI.GetComponent<GhostOwner>(entity);
                    if (owner.NetworkId == SystemAPI.GetSingleton<NetworkId>().Value)
                        continue;
                }

                // Spawn feedback locally
                var vfx = ecb.Instantiate(gameResources.OnHitFeedback);
                ecb.SetComponent(vfx, LocalTransform.FromPosition(hitEvent.ValueRO.Position));
                ecb.AddComponent(entity, new HitFeedbackEventProcessed());
                ecb.AddComponent(entity, new LifeTime()
                {
                    LifeTimeSeconds = 1f
                });
            }
        }
    }
}