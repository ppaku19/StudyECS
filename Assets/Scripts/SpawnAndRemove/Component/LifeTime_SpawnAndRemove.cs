using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct LifeTime_SpawnAndRemove : IComponentData
{
    public float Value;
}
