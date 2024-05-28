using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class LifeTimeSystem : SystemBase
{
    EntityCommandBufferSystem entityCommandBufferSystem;
    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var deltaTime = Time.DeltaTime;

        // entityInQueryIndex 는 Query로 조회된 Entities의 식별코드이다.
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref LifeTime_SpawnAndRemove lifetime) =>
        {
            lifetime.Value -= deltaTime;

            if (lifetime.Value < 0.0f)
            {
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();

        // 명령 실행 예약하기
        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
