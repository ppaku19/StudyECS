using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct RotationSpeed_SpawnAndRemove : IComponentData
{
    public float RadiansPerSecond;
}
