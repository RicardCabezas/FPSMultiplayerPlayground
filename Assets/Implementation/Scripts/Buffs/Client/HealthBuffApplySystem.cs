using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Template.CompetitiveActionMultiplayer;

namespace MultiplayerAdditions.Buffs
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBuffApplySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var lookup = SystemAPI.GetBufferLookup<Buff>(false);
            var healthLookup = SystemAPI.GetComponentLookup<Health>(false);
            lookup.Update(ref state);
            healthLookup.Update(ref state);

            foreach (var player in SystemAPI.Query<RefRO<FirstPersonPlayer>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                var e = player.ValueRO.ControlledCharacter;
                if (!lookup.HasComponent(e) || !healthLookup.HasComponent(e))
                    continue;

                var buffs = lookup[e];
                float add = 0f;
                foreach (var b in buffs)
                    if (b.Type == BuffType.ExtraHealth)
                        add += b.Value;

                var h = healthLookup[e];
                h.MaxHealth += add;
                h.CurrentHealth = math.min(h.CurrentHealth + add, h.MaxHealth);
                healthLookup[e] = h;
            }
        }
    }
}