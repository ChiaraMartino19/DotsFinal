using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerTargetAuthoring : MonoBehaviour { }

public struct PlayerTarget : IComponentData
{
    public float3 position;
}

public class PlayerTargetBaker : Baker<PlayerTargetAuthoring>
{
    public override void Bake(PlayerTargetAuthoring authoring)
    {
        var e = GetEntity(TransformUsageFlags.None);
        AddComponent(e, new PlayerTarget { position = float3.zero });
    }
}
