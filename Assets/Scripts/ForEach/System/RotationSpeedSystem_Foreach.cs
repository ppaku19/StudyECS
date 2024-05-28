using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RotationSpeedSystem_Foreach : SystemBase
{
    public float Temp1;
    public float Temp2;
    
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.
        
        // 여기에서 작업에 캡처된 지역 변수에 값을 할당합니다.
        // 나중에 실행될 때 작업을 수행하는 데 필요한 모든 것입니다.
        // 예를 들어,
        // 부동 deltaTime = Time.DeltaTime;

        // 이것은 수행할 작업 단위인 새로운 종류의 작업을 선언합니다.
        // 작업은 대상 구성 요소를 매개 변수로 사용하여 Entities.ForEach로 선언됩니다.
        // 이는 두 가지를 모두 가진 세계의 모든 엔터티를 처리한다는 의미입니다.
        // 평행이동 및 회전 구성요소. 구성 요소를 처리하도록 변경하십시오.
        // 원하는 유형.
        
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("RotationSpeed_ForEach")
            // 항상 ref 다음에 in 이 있어야한다.
            .ForEach((ref Rotation rotation, in RotationSpeed_ForEach rotationSpeed) =>
            {
                // Implement the work to perform for each entity here.
                // You should only access data that is local or that is a
                // field on this job. Note that the 'rotation' parameter is
                // marked as 'in', which means it cannot be modified,
                // but allows this job to run in parallel with other jobs
                // that want to read Rotation component data.
                // For example,
                //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;

                // 여기에서 각 엔터티에 대해 수행할 작업을 구현합니다.
                // 로컬이거나 로컬인 데이터에만 액세스해야 합니다.
                // 이 작업의 필드입니다. '회전' 매개변수는 다음과 같습니다.
                // 'in'으로 표시되어 수정할 수 없음을 의미합니다.
                // 하지만 이 작업이 다른 작업과 병렬로 실행되도록 허용합니다.
                // Rotation 구성 요소 데이터를 읽고 싶습니다.
                // 예를 들어,
                //      translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
                
                rotation.Value = math.mul(
                    math.normalize(rotation.Value),
                    quaternion.AxisAngle(math.up(), rotationSpeed.RadiansPerSecond * deltaTime));
            }).ScheduleParallel();
    }
}
