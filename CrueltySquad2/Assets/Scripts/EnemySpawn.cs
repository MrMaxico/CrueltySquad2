using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawn", menuName = "ScriptableObjects/EnemySpawn", order = 1)]
[System.Serializable]
public class EnemySpawn : ScriptableObject
{
    public GameObject enemyPrefab;
    public int spawnChance;
}
