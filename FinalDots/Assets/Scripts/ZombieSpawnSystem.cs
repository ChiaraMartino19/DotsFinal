using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ZombieSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ZombieSpawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        var spawnerEntity = SystemAPI.GetSingletonEntity<ZombieSpawner>();
        if (em.HasComponent<SpawnedOnceTag>(spawnerEntity)) return;

        var spawner = SystemAPI.GetSingleton<ZombieSpawner>();
        var rand = new Unity.Mathematics.Random(12345);

        for (int i = 0; i < spawner.count; i++)
        {
            var e = em.Instantiate(spawner.prefab);

            
            float angle = rand.NextFloat(0f, math.PI * 2f);
            float r = rand.NextFloat(0f, spawner.radius);
            float3 pos = spawner.center + new float3(math.cos(angle) * r, 0f, math.sin(angle) * r);

            em.SetComponentData(e, LocalTransform.FromPosition(pos));
            em.AddComponent<ZombieTag>(e);
            em.AddComponentData(e, new ZombieMove { speed = spawner.speed, stopDistance = spawner.stopDistance });

           
            float a = rand.NextFloat(0f, math.PI * 2f);
            float2 dir = math.normalizesafe(new float2(math.cos(a), math.sin(a)));
            float offsetRadius = rand.NextFloat(0.8f, 2.2f);

            em.AddComponentData(e, new ZombieOffset { dir = dir, radius = offsetRadius });

            
            em.AddComponentData(e, new ZombieVelocity { value = float3.zero });
        }

        em.AddComponent<SpawnedOnceTag>(spawnerEntity);
    }
}
