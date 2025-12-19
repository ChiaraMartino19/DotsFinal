using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ZombieSpawnerAuthoring : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int count = 100;
    public float3 center = new float3(-20, 0, 0);
    public float radius = 8f;
    public float speed = 6f;
    public float stopDistance = 1.2f;
}

public struct ZombieSpawner : IComponentData
{
    public Entity prefab;
    public int count;
    public float3 center;
    public float radius;
    public float speed;
    public float stopDistance;
}

public struct SpawnedOnceTag : IComponentData { }

public class ZombieSpawnerBaker : Baker<ZombieSpawnerAuthoring>
{
    public override void Bake(ZombieSpawnerAuthoring authoring)
    {
        var e = GetEntity(TransformUsageFlags.None);
        AddComponent(e, new ZombieSpawner
        {
            prefab = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic),
            count = authoring.count,
            center = authoring.center,
            radius = authoring.radius,
            speed = authoring.speed,
            stopDistance = authoring.stopDistance
        });
    }
}
