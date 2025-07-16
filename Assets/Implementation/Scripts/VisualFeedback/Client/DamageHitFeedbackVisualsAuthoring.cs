using Unity.Entities;
using UnityEngine;

namespace MultiplayerAdditions.PredictedFeedback
{
    public class DamageHitPredictedFeedbackVisualsAuthoring : MonoBehaviour
    {
        public float LifetimeSeconds = 1f;
        public bool IsNetworked;
        

        public class HitFeedbackBaker : Baker<DamageHitPredictedFeedbackVisualsAuthoring>
        {
            public override void Bake(DamageHitPredictedFeedbackVisualsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                if (authoring.IsNetworked)
                {
                    AddComponent<HitFeedbackEvent>(entity);
                }
                else
                {
                    AddComponent<DamageHitFeedback>(entity);
                    AddComponent(entity, new LifeTime
                    {
                        LifeTimeSeconds = authoring.LifetimeSeconds,
                    });
                }
            }
        }
    }
}