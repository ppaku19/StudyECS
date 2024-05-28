using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RotationSpeedSystem_IJobChunk : SystemBase
{
    // [BurstCompile] 속성을 사용하여 Burst로 Job을 컴파일합니다.상당한 속도 향상을 볼 수 있으므로 시도해 보십시오!
    [BurstCompile]
    struct RotationSpeedJob : IJobEntityBatch
    {
        public float DeltaTime;

        // Handle 필드
        public ComponentTypeHandle<Rotation> RotationTypeHandle;
        [ReadOnly] public ComponentTypeHandle<RotationSpeed_IJobEntityBatch> RotationSpeedTypeHandle;

        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            // batchInChunk 에서 아래 Handle과 같은 타입의 데이터를 NativeArray로 얻어온다.(참조)
            var chunkRotations = batchInChunk.GetNativeArray(RotationTypeHandle);
            var chunkRotationSpeeds = batchInChunk.GetNativeArray(RotationSpeedTypeHandle);

            for (int i = 0; i < batchInChunk.Count; i++)
            {
                var rotation = chunkRotations[i];
                var rotationSpeed = chunkRotationSpeeds[i];

                chunkRotations[i] = new Rotation
                {
                    Value = math.mul
                    (
                        math.normalize(rotation.Value), 
                        quaternion.AxisAngle(math.up(), rotationSpeed.RadiansPerSecond * DeltaTime)
                    )
                };
            }
        }
    }
    
    private EntityQuery m_Query;
    
    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(Rotation), ComponentType.ReadOnly<RotationSpeed_IJobEntityBatch>());
    }
    
    protected override void OnUpdate()
    {
        // 핸들 얻어오기
        var rotationType = GetComponentTypeHandle<Rotation>();
        var rotationSpeedType = GetComponentTypeHandle<RotationSpeed_IJobEntityBatch>(true);

        var job = new RotationSpeedJob()
        {
            // 필드에 데이터 전달
            RotationTypeHandle = rotationType,
            RotationSpeedTypeHandle = rotationSpeedType,
            DeltaTime = Time.DeltaTime
        };

        // 작업 예약. Dependency(의존성)을 전달하여 여러 스레드에서 쓰고, 읽기로 발생하는 경쟁 문제를 해결한다.
        Dependency = job.ScheduleParallel(m_Query, Dependency);
    }
}
