using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyTypes enemyType;
    [Space(20)]
    public IslandGenerator generator;
    public GameObject player;
    [Space(20)]
    public float speed;
    [Space(20)]
    public bool angry;
    Vector3 idleDestination;
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

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < followDistance)
        {
            angry = true;
        }
        else if (player != null && Vector3.Distance(transform.position, player.transform.position) >= followDistance)
        {
            angry = false;
        }

        if (path.Count > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[1], speed * Time.deltaTime);
        }
        else if (path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[0], speed * Time.deltaTime);
        }

        //if (!angry && path.Count == 0)
        //{
        //    idleDestination = generator.grid.RandomNode().position;
        //}

        //if (!angry)
        //{
        //    path = generator.grid.FindPath(transform.position, idleDestination);
        //}
    }

    private IEnumerator FindPath()
    {
        yield return new WaitForSeconds(pathRefreshRate);
        if (angry)
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
