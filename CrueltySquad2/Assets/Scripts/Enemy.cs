using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyTypes enemyType;
    [Space(20)]
    public IslandGenerator generator;
    public GameObject player;

    private void Start()
    {

    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }
    }

    public enum EnemyTypes
    {
        flyEnemy,
        crawlerEnemy
    }
}
