using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private string monsterTag = "Orc";
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5f;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PoolManager.Instance != null);
        yield return new WaitForSeconds(0.1f); 

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMonster()
    {
        int index = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[index].position;

        GameObject monster = PoolManager.Instance.SpawnFromPool(monsterTag, spawnPos, Quaternion.identity);

        if (monster == null)
        {
            return;
        }
    }
}
