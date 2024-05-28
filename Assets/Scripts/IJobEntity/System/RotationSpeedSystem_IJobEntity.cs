using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RotateEntityJob : IJobEntity
{
    public float DeltaTime;
    public void Execute(ref Rotation rotation, in RotationSpeed_IJobEntity speed)
    {
        rotation.Value = math.mul(math.normalize(rotation.Value),
            quaternion.AxisAngle(math.up(), speed.RadiansPerSecond * DeltaTime));
    }
}
public partial class RotationSpeedSystem_IJobEntity : SystemBase
{
    protected override void OnUpdate()
    {
        // NativeArray 초기화 배열크기, 메모리 할당
        NativeArray<Translation> test = new NativeArray<Translation>(3, Allocator.TempJob);
        
        // 구조체 생성 후 실행 예약
        new RotateEntityJob { DeltaTime = Time.DeltaTime }.ScheduleParallel();

        // 메모리 할당 해제
        test.Dispose();
    }
}
