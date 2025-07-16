using Unity.Entities;
using Unity.NetCode;
using Unity.Template.CompetitiveActionMultiplayer;

namespace MultiplayerAdditions.Buffs
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ApplyBuffRequestsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .ValueRW
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (buffRequests, player, entity) in SystemAPI
                         .Query<DynamicBuffer<BuffRequest>, RefRO<FirstPersonPlayer>>().WithEntityAccess())
            {
                var target = player.ValueRO.ControlledCharacter;
                var buffBuf = ecb.AddBuffer<Buff>(target);
                foreach (var buffRequest in buffRequests)
                {
                    //TODO: implement proper ownership logic
                    /*if (buffRequest.TargetNetworkId != entity)
                    {
                        //Can't target other players with this implementation
                        continue;
                    }*/

                    buffBuf.Add(new Buff
                        { Type = buffRequest.Type, Value = buffRequest.Value, TimeRemaining = buffRequest.Duration });
                }

                buffRequests.Clear();
            }
        }
    }
}