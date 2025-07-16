using System.Globalization;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Template.CompetitiveActionMultiplayer.Implementation
{
    /// <summary>
    /// This class creates and update the position of the players name on each character currently playing.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(ProjectilePredictionUpdateGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class HealthBarUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<GameManagedResources>();
        }

        protected override void OnUpdate()
        {
            SpawnHealthBar();
            UpdateHealthBarValue();
            UpdateHealthBarPosition();
            CleanUpHealthBar();
        }

        void SpawnHealthBar()
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .ValueRW.CreateCommandBuffer(World.Unmanaged);
            foreach (var (healthBarProxy, entity) in SystemAPI.Query<RefRO<HealthBarProxy>>()
                         .WithNone<HealthBarProxyCleanup>().WithEntityAccess())
            {
                //Obtain health
                Entity playerEntity = healthBarProxy.ValueRO.PlayerEntity;

                var health = -1f;
                if (EntityManager.HasComponent<Health>(playerEntity))
                    health = EntityManager.GetComponentData<Health>(playerEntity).CurrentHealth;

                var healthBarContained = GameManager.Instance.PlayerNameContainer; //TODO: check what is this

                //TODO: dont use this singleton, create a new one
                GameObject healthBarInstance =
                    Object.Instantiate(SystemAPI.GetSingleton<GameManagedResources>().HealthBarPrefab.Value,
                        healthBarContained.transform, true);

                var uiDocumentComponent = healthBarInstance.GetComponent<UIDocument>();
                ProgressBar progressBar = uiDocumentComponent.rootVisualElement.Q<ProgressBar>();
                progressBar.value = health;
                progressBar.title = health.ToString(CultureInfo.InvariantCulture);

                ecb.AddComponent(entity, new HealthBarProxyCleanup { UIDocumentComponent = uiDocumentComponent });
            }
        }

        void UpdateHealthBarValue()
        {
            ComponentLookup<Health> healthLookup = SystemAPI.GetComponentLookup<Health>(true);
            var ecb = SystemAPI.GetSingletonRW<EndSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                .CreateCommandBuffer(World.Unmanaged);

            foreach (var (proxy, cleanup, entity) in SystemAPI.Query<RefRO<HealthBarProxy>, HealthBarProxyCleanup>()
                         .WithEntityAccess())
            {
                var playerEntity = proxy.ValueRO.PlayerEntity;
                if (!healthLookup.HasComponent(playerEntity))
                    continue;

                var health = healthLookup[playerEntity];

                if (health.CurrentHealth <= 0f)
                {
                    // Remove the HealthBarProxy to trigger cleanup
                    ecb.RemoveComponent<HealthBarProxy>(entity);
                    continue;
                }

                var healthRatio = health.CurrentHealth / health.MaxHealth;

                var progressBar = cleanup.UIDocumentComponent.rootVisualElement.Q<ProgressBar>();
                if (progressBar != null)
                {
                    progressBar.value = healthRatio;
                    progressBar.title = health.CurrentHealth.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        
        void CleanUpHealthBar()
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingletonRW<EndSimulationEntityCommandBufferSystem.Singleton>()
                .ValueRW.CreateCommandBuffer(World.Unmanaged);
            foreach (var (cleanup, entity) in SystemAPI.Query<HealthBarProxyCleanup>().WithNone<HealthBarProxy>()
                         .WithEntityAccess())
            {
                if (cleanup.UIDocumentComponent)
                {
                    cleanup.UIDocumentComponent.rootVisualElement.Clear();
                    Object.Destroy(cleanup.UIDocumentComponent.gameObject);
                }

                ecb.RemoveComponent<HealthBarProxyCleanup>(entity);
            }
        }

        void UpdateHealthBarPosition()
        {
            if (SystemAPI.HasSingleton<MainCamera>())
            {
                Entity mainCameraEntity = SystemAPI.GetSingletonEntity<MainCamera>();
                float3 mainCameraPosition = SystemAPI.GetComponent<LocalToWorld>(mainCameraEntity).Position;

                foreach (var (ltw, cleanup) in SystemAPI.Query<RefRO<LocalToWorld>, HealthBarProxyCleanup>())
                {
                    if (cleanup.UIDocumentComponent)
                    {
                        var ltwPosition = ltw.ValueRO.Position;
                        var lookAtDirection = ltwPosition - mainCameraPosition;
                        cleanup.UIDocumentComponent.transform.SetPositionAndRotation(ltwPosition,
                            Quaternion.LookRotation(lookAtDirection));
                    }
                }
            }
        }
    }
}
