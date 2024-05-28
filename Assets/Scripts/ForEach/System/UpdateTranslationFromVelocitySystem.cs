using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial class UpdateTranslationFromVelocitySystem : SystemBase
{
    private EntityQuery query;

    protected override void OnCreate()
    {
        var description = new EntityQueryDesc()
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<Translation>()
                // ,ComponentType.ReadOnly<VelocityVector>()
            }
        };
        query = this.GetEntityQuery(description);
    }

    protected override void OnUpdate()
    {
        var updateFromVelocityJob = new UpdateTranslationFromVelocityJob();

        // Component Handle 생성
        updateFromVelocityJob.translationTypeHandle = this.GetComponentTypeHandle<Translation>(false);
        // updateFromVelocityJob.velocityTypeHandle = this.GetComponentTypeHandle<VelocityVector>(true);
        
        // 지역 변수에 할당된 값을 작업에 할당
        updateFromVelocityJob.DeltaTime = Time.DeltaTime;

        // Job 예약 (스레드 Safty 처리)
        this.Dependency = updateFromVelocityJob.ScheduleParallel(query, this.Dependency);
    }
}
