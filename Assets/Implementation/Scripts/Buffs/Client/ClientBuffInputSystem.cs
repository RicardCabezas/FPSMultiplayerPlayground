using Unity.Entities;
using Unity.NetCode;
using Unity.Template.CompetitiveActionMultiplayer;
using UnityEngine;

namespace MultiplayerAdditions.Buffs
{
    /// <summary>
    /// This is just an example made by pure convinience to be abel to quickly apply buffs
    /// Ideally, this should probably be replaced with whatever the design requires. If it's input based use a more robust generic input system,
    /// if it's based on grabbing the buffs from the world, use a system that queries for the buffs in the world and applies them, etc.
    /// </summary>
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct ClientBuffInputSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (reqBuf, player) in SystemAPI
                         .Query<DynamicBuffer<BuffRequest>, RefRO<FirstPersonPlayer>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                if (Input.GetKeyDown(KeyCode.Z))
                    reqBuf.Add(new BuffRequest
                    {
                        Type = BuffType.SpeedMultiplier, 
                        //Again, just for convenience, this should be replaced with config
                        Value = 1.5f, 
                        Duration = 5f
                    });
            }
        }
    }
}