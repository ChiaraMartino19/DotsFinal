using Unity.Entities;
using Unity.Mathematics;

public struct ZombieOffset : IComponentData
{
    public float2 dir;    
    public float radius; 
}
