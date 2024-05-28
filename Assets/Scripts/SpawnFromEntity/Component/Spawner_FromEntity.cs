using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Spawner_FromEntity : IComponentData
{
    public int CountX;
    public int CountY;
    public Entity Prefab;
}
