using Unity.Entities;
using Unity.NetCode;

namespace MultiplayerAdditions.Buffs
{
    [GhostComponent]
    public struct Buff : IBufferElementData
    {
        public BuffType Type;
        public float Value;
        public float TimeRemaining;
    }
}