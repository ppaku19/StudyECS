using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public partial class SpawnerSystem_SpawnAndRemove : SystemBase
{
    BeginInitializationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        // 시스템이 생성되면 OnCreate 호출
        // BeginInitializationEntityCommandBufferSystem을 가져온다. 
        entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

    }

    protected override void OnUpdate()
    {
        // 병렬 쓰기 작업에서 명령을 예약할 예정이므로 AsParallelWriter();를 설정한다.
        var commandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("SpawnerSystem_SpawnAndRemove")
            // 1, 2번 매개변수는 기본값이다. 3번째는 동기식으로 처리한다는 의미이다. 
            // 대충 이런게 있다 정도로만 이해하자.
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in Spawner_SpawnAndRemove spawner, in LocalToWorld location) =>
            {
                var random = new Unity.Mathematics.Random(1);
                
                for (var x = 0; x < spawner.CountX; x++)
                {
                    for (var y = 0; y < spawner.CountY; y++)
                    {
                        // entity 생성 예약
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.Prefab);

                        // 엔티티가 생성될 위치를 결정한다.
                        var position = math.transform(location.Value, 
                            new float3(x * 1.3F, noise.cnoise(new float2(x, y) * 0.21F) * 2, y * 1.3F));

                        // 생성한 Entity의 위치를 position위치로 변경하는 명령을 예약한다. 
                        commandBuffer
                            .SetComponent(entityInQueryIndex, instance, new Translation
                                { Value = position });
                        
                        // 회전 속도
                        commandBuffer
                            .SetComponent(entityInQueryIndex, instance, new RotationSpeed_SpawnAndRemove
                                { RadiansPerSecond = math.radians(random.NextFloat(25.0F, 90.0F)) });
                        
                        // 생존할 시간
                        commandBuffer
                            .SetComponent(entityInQueryIndex, instance, new LifeTime_SpawnAndRemove
                                { Value = random.NextFloat(10.0F, 20.0F) });
                    }
                }                
                
                // 스폰 Entity를 제거한다. 제거하지 않으면 onUpdate에 의해 Cube가 계속 생성된다.
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        // 위의 작업이 완료되면
        // 예약한 명령을 다음 프레임에서 BeginInitializationEntityCommandBufferSystem 호출될때 처리하라고 등록한다.
        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}