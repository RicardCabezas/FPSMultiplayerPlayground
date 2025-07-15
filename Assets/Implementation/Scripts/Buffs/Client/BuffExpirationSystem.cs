using Unity.Collections;
using Unity.Entities;

namespace MultiplayerAdditions.Buffs
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct BuffExpirationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var buffer in SystemAPI
                         .Query<DynamicBuffer<Buff>>())
            {
                // local copy allows mutation
                // also, is a reference type so changes affect the data
                var buffs = buffer;
                for (int i = buffs.Length - 1; i >= 0; --i)
                {
                    var b = buffs[i];
                    b.TimeRemaining -= dt;

                    if (b.TimeRemaining <= 0f)
                        buffs.RemoveAtSwapBack(i);
                    else
                        buffs[i] = b;
                }
            }
        }
    }
}