using Unity.Entities;

public struct ZombieTag : IComponentData { }

public struct ZombieMove : IComponentData
{
    public float speed;
    public float stopDistance;
}
