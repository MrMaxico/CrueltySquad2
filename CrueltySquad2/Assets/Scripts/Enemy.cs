using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyTypes enemyType;
    [Space(20)]
    public IslandGenerator generator;
    public GameObject player;
    [Space(20)]
    public float speed;
    [Space(20)]
    public float followDistance;
    public List<Vector3> path;
    public float pathRefreshRate;

    private void Start()
    {
        path = new List<Vector3> { };

        StartCoroutine(FindPath());
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        if (path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[1], speed * Time.deltaTime);
        }
    }

    private IEnumerator FindPath()
    {
        yield return new WaitForSeconds(pathRefreshRate);
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < followDistance)
        {
            path = generator.grid.FindPath(transform.position, player.transform.position);
        }
        StartCoroutine(FindPath());
    }

    public enum EnemyTypes
    {
        flyEnemy,
        crawlerEnemy
    }
}
