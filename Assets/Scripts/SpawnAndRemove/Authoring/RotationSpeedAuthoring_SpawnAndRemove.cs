using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[ConverterVersion("joe", 1)]
public class RotationSpeedAuthoring_SpawnAndRemove : MonoBehaviour, IConvertGameObjectToEntity
{
    public float DegreesPerSecond = 360;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new RotationSpeed_SpawnAndRemove() { RadiansPerSecond = math.radians(DegreesPerSecond) });
        
        // LifeTime Component를 Cube에 추가한다.
        dstManager.AddComponentData(entity, new LifeTime_SpawnAndRemove { Value = 0.0F });
    }
}
