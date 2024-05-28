using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnerAuthoring_FromEntity : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject Prefab;
    public int CountX;
    public int CountY;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new Spawner_FromEntity
        {
            CountX = CountX,
            CountY = CountY,
            Prefab = conversionSystem.GetPrimaryEntity(Prefab)
        };

        dstManager.AddComponentData(entity, spawnerData);
    }
}
