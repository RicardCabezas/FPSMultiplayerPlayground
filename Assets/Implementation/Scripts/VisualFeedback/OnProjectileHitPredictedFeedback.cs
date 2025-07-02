using MultiplayerAdditions.PredictedFeedback;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Template.CompetitiveActionMultiplayer;
using Unity.Transforms;

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
                if (projectile.ValueRO.HasHit != 1 || projectile.ValueRO.HitEntity == Entity.Null)
                {
                }
                else
                {
                    Entity fxEntity = ecb.Instantiate(gameResources.OnHitFeedback);
                
                    //Since the Hit entity doesnt have the position of the hit I need to do this workaround
                    if (SystemAPI.HasComponent<LocalToWorld>(projectile.ValueRO.HitEntity))
                    {
                        float3 hitPos = SystemAPI.GetComponent<LocalToWorld>(projectile.ValueRO.HitEntity).Position;

                        ecb.SetComponent(fxEntity, LocalTransform.FromPositionRotation(
                            hitPos, quaternion.identity));
                    }

                    ecb.AddComponent<DamageHitFeedback>(entity);
                }
            }
        }
    }
