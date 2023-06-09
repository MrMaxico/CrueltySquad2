using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Health healthManager;
    [SerializeField] int spawnCap;
    int spawnedEnemies;
    [Space(20)]
    [SerializeField] EnemySpawn[] enemySpawns;
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

    private void Update()
    {
        if (!healthManager.IsAlive())
        {
            generator.Teleporter().GetComponent<Teleporter>().spawnersLeft -= 1;
            //add better looking death of spawner later
            Destroy(this.gameObject);
        }
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
            GameObject spawnedEnemy = Instantiate(RandomEnemySpawn().enemyPrefab, transform.position, Quaternion.identity);
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

    EnemySpawn RandomEnemySpawn()
    {
        List<EnemySpawn> enemySpawnChanceList = new();
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
