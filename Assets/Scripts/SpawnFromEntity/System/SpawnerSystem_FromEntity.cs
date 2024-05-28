using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

// default(UpdateInGroup), ext(UpdateInGroup, UpdateBefore, UpdateAfter, DisableAutoCreation)
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class SpawnerSystem_FromEntity : SystemBase
{
    [BurstCompile]
    struct SetSpawnedTranslation : IJobParallelFor
    {
        // 보통은 병렬 작업에서 ComponentDataFromEntity에 쓰기 작업을 할 수 없다.
        // 병렬 쓰기를 가능하게 한다. 즉, IJobParallelFor는 인덱스 값으로 쓰기를 하므로 경쟁 문제가 발생하지 않으므로 사용한다. 
        [NativeDisableParallelForRestriction]
        public ComponentDataFromEntity<Translation> TranslationFromEntity;

        public NativeArray<Entity> Entities;

        // 로컬 좌표
        public float4x4 LocalToWorld;
        public int Stride;

        public void Execute(int index)
        {
            var entity = Entities[index];
            var y = index / Stride;
            var x = index - (y * Stride);

            // Entity의 좌표를 변경한다.
            TranslationFromEntity[entity] = new Translation()
            {
                Value = math.transform(LocalToWorld, new float3(x * 1.3F, noise.cnoise(new float2(x, y) * 0.21F) * 2, y * 1.3F))
            };
        }
    }
    
    protected override void OnUpdate()
    {
        // WithStructuralChanges 버스트를 비활성하여 함수 내에서 엔티티 데이터를 구조적으로 변경할 수 있게 해준다.
        // WithStructuralChanges 보단 EntityCommandBuffer를 사용하는 것이 성능상 더 좋다. 
        Entities.WithStructuralChanges().ForEach((Entity entity, int entityInQueryIndex,
            in Spawner_FromEntity spawnerFromEntity, in LocalToWorld spawnerLocalToWorld) =>
        {

            // Job 이 끝날때까지 메인쓰레드가 대기한다.
            Dependency.Complete();

            var spawnedCount = spawnerFromEntity.CountX * spawnerFromEntity.CountY;

            // NativeArray<Entity> 초기화
            var spawnedEntities =
                new NativeArray<Entity>(spawnedCount, Allocator.TempJob); 

            // spawnedEntities 크기만큼 Entity를 생성하고 spawnedEntities에 생성한 Entity를 넣습니다.
            EntityManager.Instantiate(spawnerFromEntity.Prefab, spawnedEntities);
    
            // Spawner Entity를 제거합니다. (안 그러면 매프레임마다 Entity를 생성함)
            EntityManager.DestroyEntity(entity);

            var translationFromEntity = GetComponentDataFromEntity<Translation>();
            var setSpawnedTranslationJob = new SetSpawnedTranslation
            {
                TranslationFromEntity = translationFromEntity,
                Entities = spawnedEntities,
                LocalToWorld = spawnerLocalToWorld.Value,
                Stride = spawnerFromEntity.CountX
            };


            // spawnedCount 수행할 반복횟수
            // 두번째 매개변수는 배치 크기이다. 보통 32 또는 64를 사용하며, 매우 큰 Job일 경우 1를 쓰는 것이 좋을 수 있다.
            Dependency = setSpawnedTranslationJob.Schedule(spawnedCount, 64, Dependency);
            Dependency = spawnedEntities.Dispose(Dependency);
        }).Run();
    }
}
