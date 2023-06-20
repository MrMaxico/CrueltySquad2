using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public EnemyTypes enemyType;
    [Space(20)]
    public IslandGenerator generator;
    public Spawner spawner;
    public GameObject player;
    [Space(20)]
    public float speed;
    [Space(20)]
    public bool angry;
    Vector3 idleDestination;
    public float followDistance;
    public List<Vector3> path;
    public float pathRefreshRate;
    float pathRefreshTimer;
    [Space(20)]
    public LayerMask pLayer;
    public float damage;
    public Vector3 relativeHitPosition;
    public float attackSpeed;
    public float attackTimer;
    [Space(20)]
    [Tooltip("You only need this variable if the enemy is an fly enemy")]
    [SerializeField] float flyEnemyFlightHeight;
    [Space(20)]
    public Renderer renderer;
    public bool activeIdle;

    private void Start()
    {
        path = new List<Vector3> { };

        FindPath();
    }

    private void Update()
    {
        pathRefreshTimer += Time.deltaTime;

        if (pathRefreshTimer > pathRefreshRate)
        {
            pathRefreshTimer = 0;
            FindPath();
        }

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
        
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (path.Count > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[1], speed * Time.deltaTime);
        }
        else if (angry && path.Count == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position - transform.forward * (relativeHitPosition.z * 1.7f), speed * Time.deltaTime);
            if (enemyType == EnemyTypes.flyEnemy)
            {
                if (attackTimer <= 0)
                {
                    attackTimer = attackSpeed;
                    StartCoroutine(RunAnimation());
                }
            }
            else if (enemyType == EnemyTypes.crawlerEnemy)
            {
                if (attackTimer <= 0)
                {
                    attackTimer = attackSpeed;
                    Vector3 spherePosition = transform.position + transform.forward * relativeHitPosition.z +
                                             transform.right * relativeHitPosition.x +
                                             transform.up * relativeHitPosition.y;
                    if (Physics.CheckSphere(spherePosition, .3f, pLayer))
                    {
                        float playerHealthBeforeDamage = player.GetComponent<Health>().GetHealth();
                        player.GetComponent<Health>().Damage(damage);
                        Debug.Log($"Damaged player. Health went from {playerHealthBeforeDamage} to {player.GetComponent<Health>().GetHealth()}");
                    }
                }
            }
        }
        //else if (path.Count > 0)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, path[0], speed * Time.deltaTime);
        //}

        //if (!angry && path.Count == 0) //
        if (!angry && path.Count == 1)
        {
            idleDestination = generator.grid.RandomNode().position;
        }

        if (renderer.isVisible && activeIdle == false)
        {
            activeIdle = true;
        }
        else if (!renderer.isVisible && activeIdle == true)
        {
            activeIdle = false;
        }
    }

    private void FindPath()
    {
        Profiler.BeginSample($"Finding path");
        if (angry)
        {
            path = generator.grid.FindPath(transform.position, player.transform.position);
            transform.transform.LookAt(player.transform);
        }
        else if (!angry && activeIdle)
        {
            path = generator.grid.FindPath(transform.position, idleDestination);
        }
        Profiler.EndSample();

        if (enemyType == EnemyTypes.flyEnemy)
        {
            if (activeIdle || angry)
            {
                Profiler.BeginSample("Lifting path up in the air for flying enemies");
                float averageHeight = 0;
                foreach (Vector3 waypoint in path)
                {
                    averageHeight += waypoint.y;
                }

                averageHeight /= path.Count;

                for (int i = 0; i < path.Count; i++)
                {
                    if (path[i].y > averageHeight)
                    {
                        averageHeight = path[i].y + (flyEnemyFlightHeight / 3);
                    }
                }

                for (int i = 0; i < path.Count; i++)
                {
                    //path[i] = new Vector3(path[i].x, averageHeight + flyEnemyFlightHeight, path[i].z);
                    Vector3 newWaypoint;
                    newWaypoint.x = path[i].x;
                    newWaypoint.y = averageHeight + flyEnemyFlightHeight;
                    newWaypoint.z = path[i].z;
                    path[i] = newWaypoint;
                }
                Profiler.EndSample();
            }
        }
    }

    public IEnumerator RunAnimation()
    {
        if (enemyType == EnemyTypes.flyEnemy)
        {
            GetComponent<Animator>().SetTrigger("isAttacking");
            yield return new WaitForEndOfFrame();
            GetComponent<Animator>().ResetTrigger("isAttacking");
        }
    }

    public void DamagePlayer()
    {
        if (enemyType == EnemyTypes.flyEnemy)
        {
            Vector3 spherePosition = transform.position + transform.forward * relativeHitPosition.z +
                                                     transform.right * relativeHitPosition.x +
                                                     transform.up * relativeHitPosition.y;
            if (Physics.CheckSphere(spherePosition, .3f, pLayer))
            {
                float playerHealthBeforeDamage = player.GetComponent<Health>().GetHealth();
                player.GetComponent<Health>().Damage(damage);
                Debug.Log($"Damaged player. Health went from {playerHealthBeforeDamage} to {player.GetComponent<Health>().GetHealth()}");
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 spherePosition = transform.position + transform.forward * relativeHitPosition.z +
                                 transform.right * relativeHitPosition.x +
                                 transform.up * relativeHitPosition.y;
        Gizmos.DrawSphere(spherePosition, .3f);
    }

    public enum EnemyTypes
    {
        flyEnemy,
        crawlerEnemy
    }
}
