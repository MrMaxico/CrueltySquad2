using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool lvlOneSpawner;
    public GameObject lvlTwoSpawner;
    public GameObject lvlThreeSpawner;
    [Space(20)]
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
        if (Teleporter.islandNumber >= 3 && Teleporter.islandNumber < 10 && lvlOneSpawner)
        {
            GameObject spawnedStructure = Instantiate(lvlTwoSpawner, transform.position, transform.rotation);
            spawnedStructure.GetComponent<Spawner>().generator = generator;
            Destroy(this.gameObject);
        }
        else if (Teleporter.islandNumber >= 10 && lvlOneSpawner)
        {
            GameObject spawnedStructure = Instantiate(lvlThreeSpawner, transform.position, transform.rotation);
            spawnedStructure.GetComponent<Spawner>().generator = generator;
            Destroy(this.gameObject);
        }
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
            GameObject spawnedEnemy = Instantiate(RandomEnemySpawn(), transform.position, Quaternion.identity);
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

    GameObject RandomEnemySpawn()
    {
        List<GameObject> enemySpawnChanceList = new();
        foreach (RandomVariable enemySpawn in enemySpawns)
        {
            for (int i = 0; i < enemySpawn.spawnChance; i++)
            {
                enemySpawnChanceList.Add(enemySpawn.GameObject);
            }
        }
        return enemySpawnChanceList[Mathf.RoundToInt(Random.Range(0, enemySpawnChanceList.Count))];
    }
}
