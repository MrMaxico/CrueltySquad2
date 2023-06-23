using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Health healthManager;
    [SerializeField] int spawnCap;
    int spawnedEnemies;
    [Space(20)]
    [SerializeField] RandomVariable[] enemySpawns;
    [SerializeField] float preperationTime;
    bool preparing = true;
    [SerializeField] float spawnInterval;
    [SerializeField] int onDeathEnemySpawnAmount;
    [Space(20)]
    public IslandGenerator generator;

    private void Start()
    {
        generator.Teleporter().GetComponent<Teleporter>().spawnersLeft += 1;
        StartCoroutine(SpawnCycle());
    }

    public void OnEnemyKill()
    {
        spawnedEnemies--;
    }

    IEnumerator SpawnCycle()
    {
        if (preparing)
        {
            yield return new WaitForSeconds(preperationTime);
            preparing = false;
        }
        if (spawnedEnemies < spawnCap)
        {
            GameObject spawnedEnemy = Instantiate(RandomEnemySpawn().GameObject, transform.position, Quaternion.identity);
            spawnedEnemy.GetComponent<Enemy>().generator = generator;
            spawnedEnemy.GetComponent<Enemy>().spawner = this;
            spawnedEnemies++;
        }
        yield return new WaitForSeconds(spawnInterval);
        if (healthManager.IsAlive())
        {
            StartCoroutine(SpawnCycle());
        }
    }

    RandomVariable RandomEnemySpawn()
    {
        List<RandomVariable> enemySpawnChanceList = new();
        foreach (RandomVariable enemySpawn in enemySpawns)
        {
            for (int i = 0; i < enemySpawn.spawnChance; i++)
            {
                enemySpawnChanceList.Add(enemySpawn);
            }
        }
        return enemySpawnChanceList[Mathf.RoundToInt(Random.Range(0, enemySpawnChanceList.Count))];
    }
}
