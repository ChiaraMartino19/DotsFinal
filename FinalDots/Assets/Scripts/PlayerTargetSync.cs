using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerTargetSync : MonoBehaviour
{
    EntityManager em;
    EntityQuery query;
    Entity targetEntity;
    bool ready;

    void Awake()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        query = em.CreateEntityQuery(ComponentType.ReadWrite<PlayerTarget>());
    }

    void Update()
    {
        if (!ready)
        {
            if (query.IsEmptyIgnoreFilter) return;
            targetEntity = query.GetSingletonEntity();
            ready = true;
        }

        var t = em.GetComponentData<PlayerTarget>(targetEntity);
        var p = transform.position;
        t.position = new float3(p.x, 0f, p.z); 
        em.SetComponentData(targetEntity, t);
    }
}
