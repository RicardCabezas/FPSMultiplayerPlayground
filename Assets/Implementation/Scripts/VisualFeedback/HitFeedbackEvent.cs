using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace MultiplayerAdditions.PredictedFeedback
{
    [GhostComponent]
    public struct HitFeedbackEvent : IComponentData
    {
        [GhostField(Quantization=100)]
        public float3 Position;
    }
}