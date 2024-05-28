using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[Serializable]
public struct UpdateTranslationFromVelocityJob : IJobEntityBatch
{
    // sample...
    // public ComponentTypeHandle<VelocityVector> velocityTypeHandle;
    public ComponentTypeHandle<Translation> translationTypeHandle;
    public float DeltaTime;
    // public ComponentDataFromEntity<RotationSpeed_ForEach> t2; // 모든 Entity 데이터 조회 가능하나 성능이 좋지 못하다. 주의해서 사용
    // public BufferFromEntity<T> t3; // 모든 Entity 데이터 조회 가능하나 성능이 좋지 못하다. 주의해서 사용

    // Add fields to your component here. Remember that:
    //
    // * A component itself is for storing data and doesn't 'do' anything.
    //
    // * To act on the data, you will need a System.
    //
    // * Data in a component must be blittable, which means a component can
    //   only contain fields which are primitive types or other blittable
    //   structs; they cannot contain references to classes.
    //
    // * You should focus on the data structure that makes the most sense
    //   for runtime use here. Authoring Components will be used for 
    //   authoring the data in the Editor.

    public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
    {
        // NativeArray<VelocityVector> velocities = batchInChunk.GetNativeArray(velocityTypeHandle);
        NativeArray<Translation> translations = batchInChunk.GetNativeArray(translationTypeHandle);

        for (int i = 0; i < batchInChunk.Count; i++)
        {
        }
    }
}
