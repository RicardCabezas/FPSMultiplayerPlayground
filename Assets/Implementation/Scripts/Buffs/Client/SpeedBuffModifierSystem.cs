using Unity.Entities;
using Unity.NetCode;
using Unity.Template.CompetitiveActionMultiplayer;

namespace MultiplayerAdditions.Buffs
{
// Presentation: scales movement input by speed buffs
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct SpeedBuffModifierSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var lookup = SystemAPI.GetBufferLookup<Buff>(isReadOnly: true);

            foreach (var (firstPersonPlayerCommands, entity) in SystemAPI
                         .Query<RefRW<FirstPersonPlayerCommands>>()
                         .WithAll<GhostOwnerIsLocal>()
                         .WithEntityAccess())
            {
                var multiplier = 1f;

                if (lookup.HasBuffer(entity))
                {
                    var buffBuf = lookup[entity];

                    foreach (var b in buffBuf)
                    {
                        if (b.Type == BuffType.SpeedMultiplier)
                        {
                            multiplier *= b.Value;
                        }
                    }
                }

                // Not really great, what should be multiplied is a speed data that should be basically (from config),
                // later once the multiplier goes away it should override the speed value with config value again
                firstPersonPlayerCommands.ValueRW.MoveInput *= multiplier;
            }
        }
    }

}