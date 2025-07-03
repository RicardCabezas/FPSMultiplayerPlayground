using Unity.Entities;
using UnityEngine;

namespace MultiplayerAdditions.PredictedFeedback
{
    public class DamageHitFeedbackVisualsAuthoring : MonoBehaviour
    {
        public float Lifetime = 1f;

        class Baker : Baker<DamageHitFeedbackVisualsAuthoring>
        {
            public override void Bake(DamageHitFeedbackVisualsAuthoring authoring)
            {
                Entity selfEntity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);

                AddComponent(selfEntity, new DamageHitFeedback
                {
                    LifeTime = authoring.Lifetime,
                });
            }
        }
    }
}