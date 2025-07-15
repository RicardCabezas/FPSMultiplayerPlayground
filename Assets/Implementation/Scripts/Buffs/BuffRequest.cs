using Unity.Entities;
using Unity.NetCode;

namespace MultiplayerAdditions.Buffs
{
    [GhostComponent]
    public struct BuffRequest : IBufferElementData
    {
        [GhostField]
        public int TargetNetworkId;
        [GhostField]
        public BuffType Type;
        [GhostField(Quantization=100)]
        public float Value;
        [GhostField(Quantization=100)]
        public float Duration;
    }
}