using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnFromMonoBehaviour : MonoBehaviour
{
    public GameObject Prefab;
    public int CountX = 100;
    public int CountY = 100;

    private void Start()
    {
        // 1. Settings 값 가져오기
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        // 2. Prefab을 Entity 형태로 만들기
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings);

        // 3. EntityManager 가져오기
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int x = 0; x < CountX; x++)
        {
            for (int y = 0; y < CountY; y++)
            {
                var instance = entityManager.Instantiate(entity); // Entity 생성

                // instance의 Translation 값 변경
                var position = transform.TransformPoint(new float3(x * 1.3f, noise.cnoise(new float2(x, y) * 0.21f) * 2, y * 1.3f));
                entityManager.SetComponentData(instance, new Translation { Value = position});
            }
        }
    }
}
