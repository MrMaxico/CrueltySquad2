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
    public EnemyStats enemyStats;
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
    public Vector3 relativeHitPosition;
    public float attackDistanceMultiplyer;
    public float attackSpeed;
    public float attackTimer;
    [Header("Spitter only")]
    public float spitDistance;
    public float spitSpeed;
    [Space(20)]
    [Tooltip("You only need this variable if the enemy is a fly enemy")]
    [SerializeField] float flyEnemyFlightHeight;
    [Space(20)]
    public Renderer renderer;
    public bool activeIdle;

    private void Start()
    {
        path = new List<Vector3> { };

        if (enemyType == EnemyTypes.lootJalla)
        {
            generator = FindObjectOfType<IslandGenerator>();
        }
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

        // Check if the enemy is not angry and there are no more nodes in the path
        // Set the idle destination to a random node on the grid
        if (!angry && path.Count <= 1)
        {
            idleDestination = generator.grid.RandomNode().position;
        }

        // Check if the renderer is currently visible and set activeIdle accordingly
        if (renderer.isVisible && !activeIdle)
        {
            activeIdle = true;
        }
        else if (!renderer.isVisible && activeIdle)
        {
            activeIdle = false;
        }

        // If the player is not assigned and the enemy type is not "lootJalla", find the player
        if (player == null && enemyType != EnemyTypes.lootJalla)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        // Check the distance between the enemy and the player to determine if the enemy should be angry or not
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < followDistance)
        {
            angry = true;
        }
        else if (player != null && Vector3.Distance(transform.position, player.transform.position) >= followDistance)
        {
            angry = false;
        }

        attackTimer -= Time.deltaTime;

        // Move towards the second node in the path if there are more than one nodes
        if (path.Count > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[1], speed * Time.deltaTime);
        }

        if (angry && path.Count <= 1 && enemyType != EnemyTypes.spitter)
        {
            // Calculate the goal position for the enemy when it is angry and about to attack the player
            Vector3 goalPosition = player.transform.position - transform.forward * (relativeHitPosition.z * attackDistanceMultiplyer);
            goalPosition.y = path[0].y;
            transform.position = Vector3.MoveTowards(transform.position, goalPosition, speed * Time.deltaTime);

            if (attackTimer <= 0)
            {
                attackTimer = attackSpeed;
                StartCoroutine(RunAnimation());
            }
        }
        else if (angry && path.Count <= 1 && enemyType == EnemyTypes.spitter)
        {
            if (attackTimer <= 0)
            {
                attackTimer = attackSpeed;
                StartCoroutine(RunAnimation());
            }
        }
    }

    private void FindPath()
    {
        Profiler.BeginSample($"Finding path");
        if (angry)
        {
            if (enemyType != EnemyTypes.spitter)
            {
                path = generator.grid.FindPath(transform.position, player.transform.position);
            }
            else
            {
                path = generator.grid.FindPath(transform.position, player.transform.position - transform.forward * spitDistance);
            }

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
        else if (enemyType == EnemyTypes.crawlerEnemy)
        {
            GetComponent<Animator>().SetTrigger("Attack2");
            yield return new WaitForEndOfFrame();
            GetComponent<Animator>().ResetTrigger("Attack2");
        }
        else if (enemyType == EnemyTypes.spitter)
        {
            GetComponent<Animator>().SetTrigger("Attack1");
            yield return new WaitForEndOfFrame();
            GetComponent<Animator>().ResetTrigger("Attack1");
        }
    }

    public void DamagePlayer()
    {
        if (enemyType == EnemyTypes.flyEnemy)
        {
            Vector3 spherePosition = transform.position + transform.forward * relativeHitPosition.z +
            transform.right * relativeHitPosition.x +
            transform.up * relativeHitPosition.y;
            if (Physics.CheckSphere(spherePosition, .5f, pLayer))
            {
                float playerHealthBeforeDamage = player.GetComponent<Health>().GetHealth();
                player.GetComponent<Health>().Damage(enemyStats.damage);
                Debug.Log($"Damaged player. Health went from {playerHealthBeforeDamage} to {player.GetComponent<Health>().GetHealth()}");
            }
        }

        if (enemyType == EnemyTypes.crawlerEnemy)
        {
            Vector3 spherePosition = transform.position + transform.forward * relativeHitPosition.z +
            transform.right * relativeHitPosition.x +
            transform.up * relativeHitPosition.y;
            if (Physics.CheckSphere(spherePosition, .3f, pLayer))
            {
                float playerHealthBeforeDamage = player.GetComponent<Health>().GetHealth();
                player.GetComponent<Health>().Damage(enemyStats.damage);
                Debug.Log($"Damaged player. Health went from {playerHealthBeforeDamage} to {player.GetComponent<Health>().GetHealth()}");
            }
        }
    }

    public void Spit(GameObject spit)
    {
        Vector3 bulletSpawnPosition = transform.position + transform.forward * relativeHitPosition.z +
                                 transform.right * relativeHitPosition.x +
                                 transform.up * relativeHitPosition.y;
        GameObject spittenObject = Instantiate(spit, bulletSpawnPosition, Quaternion.LookRotation(player.transform.position - transform.position));
        spittenObject.GetComponent<Bullet>().spitSpeed = spitDistance;
        spittenObject.GetComponent<Bullet>().player = player;
        spittenObject.GetComponent<Bullet>().damage = enemyStats.damage;
    }

    public void OnDrawGizmos()
    {
        // Draw a yellow sphere to visualize the hit position of the enemy
        Gizmos.color = Color.yellow;
        Vector3 spherePosition = transform.position + transform.forward * relativeHitPosition.z +
                                 transform.right * relativeHitPosition.x +
                                 transform.up * relativeHitPosition.y;
        Gizmos.DrawSphere(spherePosition, .5f);
    }

    public enum EnemyTypes
    {
        flyEnemy,
        crawlerEnemy,
        lootJalla,
        spitter
    }
}
