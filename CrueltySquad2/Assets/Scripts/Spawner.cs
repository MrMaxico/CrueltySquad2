using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Health healthManager;
    [Space(20)]
    [SerializeField] EnemySpawn[] enemySpawns;
    [SerializeField] float preperationTime;
    bool preparing = true;
    [SerializeField] float spawnInterval;
    [SerializeField] int onDeathEnemySpawnAmount;

    private void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle()
    {
        if (preparing)
        {
            yield return new WaitForSeconds(preperationTime);
            preparing = false;
        }
        Instantiate(RandomEnemySpawn().enemyPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(spawnInterval);
        if (healthManager.IsAlive())
        {
            StartCoroutine(SpawnCycle());
        }
    }

    EnemySpawn RandomEnemySpawn()
    {
        List<EnemySpawn> enemySpawnChanceList = new List<EnemySpawn>();
        foreach (EnemySpawn enemySpawn in enemySpawns)
        {
            for (int i = 0; i < enemySpawn.spawnChance; i++)
            {
                enemySpawnChanceList.Add(enemySpawn);
            }
        }
        return enemySpawnChanceList[Mathf.RoundToInt(Random.Range(0, enemySpawnChanceList.Count))];
    }
}
