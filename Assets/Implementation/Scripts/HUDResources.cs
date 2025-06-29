using Unity.Entities;
using UnityEngine;

namespace Unity.Template.CompetitiveActionMultiplayer.Implementation
{
    public struct HUDResources : IComponentData
    {
        public UnityObjectRef<GameObject> HealthBarUIPrefab;
        public UnityObjectRef<GameObject> DeathScreenUIPrefab;
    }
}