using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ZombieChaseSystem : ISystem
{
    NativeParallelMultiHashMap<int, float3> grid;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTarget>();
    }

    public void OnDestroy(ref SystemState state)
    {
        if (grid.IsCreated) grid.Dispose();
    }

    static int Hash(int2 cell)
    {
     
        return cell.x * 73856093 ^ cell.y * 19349663;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        float3 playerPos = SystemAPI.GetSingleton<PlayerTarget>().position;

        
        const float groundY = 0.5f;        
        const float cellSize = 1.25f;     
        const float desiredSpacing = 1.0f; 
        const float separationForce = 12f;
        const float targetForce = 6f;
        const float maxSpeed = 6f;

        playerPos.y = groundY;

      
        int zombieCount = SystemAPI.QueryBuilder().WithAll<ZombieTag>().Build().CalculateEntityCount();
        int neededCap = math.max(1024, zombieCount * 2);

        if (!grid.IsCreated || grid.Capacity < neededCap)
        {
            if (grid.IsCreated) grid.Dispose();
            grid = new NativeParallelMultiHashMap<int, float3>(neededCap, Allocator.Persistent);
        }
        grid.Clear();

        float invCell = 1f / math.max(0.0001f, cellSize);

        foreach (var tr in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<ZombieTag>())
        {
            float3 p = tr.ValueRO.Position;
            int2 cell = new int2((int)math.floor(p.x * invCell), (int)math.floor(p.z * invCell));
            grid.Add(Hash(cell), p);
        }

        float desiredSq = desiredSpacing * desiredSpacing;

        
        foreach (var (tr, mv, off, vel) in SystemAPI
                     .Query<RefRW<LocalTransform>, RefRO<ZombieMove>, RefRO<ZombieOffset>, RefRW<ZombieVelocity>>()
                     .WithAll<ZombieTag>())
        {
            float3 pos = tr.ValueRO.Position;
            pos.y = groundY;

            
            float3 slotTarget = playerPos + new float3(off.ValueRO.dir.x, 0f, off.ValueRO.dir.y) * off.ValueRO.radius;
            slotTarget.y = groundY;

            float3 toTarget = slotTarget - pos;
            toTarget.y = 0f;

            float3 accel = math.normalizesafe(toTarget) * targetForce;

           
            float3 sep = float3.zero;

            int2 myCell = new int2((int)math.floor(pos.x * invCell), (int)math.floor(pos.z * invCell));

            
            for (int dz = -1; dz <= 1; dz++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    int2 c = myCell + new int2(dx, dz);
                    int h = Hash(c);

                    if (grid.TryGetFirstValue(h, out float3 other, out var it))
                    {
                        do
                        {
                            float3 d = pos - other;
                            d.y = 0f;

                            float distSq = math.lengthsq(d);

                            
                            if (distSq > 0.0001f && distSq < desiredSq)
                            {
                                float dist = math.sqrt(distSq);
                                float3 pushDir = d / math.max(dist, 0.0001f);
                                sep += pushDir * (desiredSpacing - dist);
                            }
                        }
                        while (grid.TryGetNextValue(out other, ref it));
                    }
                }

            accel += sep * separationForce;

           
            float3 v = vel.ValueRO.value;
            v += accel * dt;

          
            float sp = math.length(v);
            if (sp > maxSpeed) v = v / sp * maxSpeed;

            pos += v * dt;
            pos.y = groundY;

            vel.ValueRW.value = v;
            tr.ValueRW.Position = pos;

            
            if (math.lengthsq(v) > 1e-4f)
                tr.ValueRW.Rotation = quaternion.LookRotationSafe(new float3(v.x, 0f, v.z), math.up());
        }
    }
}
