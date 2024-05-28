using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RotationSpeedSystem_SpawnAndRemove : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .WithName("RotationSpeedSystem_SpawnAndRemove")
            .ForEach((ref Rotation rotation, in RotationSpeed_SpawnAndRemove rotSpeedSpawnAndRemove) =>
            {
             
                rotation.Value = math.mul(math.normalize(rotation.Value), 
                    quaternion.AxisAngle(math.up(), rotSpeedSpawnAndRemove.RadiansPerSecond * deltaTime));

            }).ScheduleParallel();
    }
}