using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum HealthType {
    Enemy,
    Player,
    prop,
}
public class Health : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    public float shield;
    public float maxShield;
    public float xpOnDeath = 10;
    [Header("Type of user", order = 0)]
    public HealthType healthType;
    [Header("Death (Only for Enemies)", order = 1)]
    public GameObject deathSplash;
    public WeightedRandomList<GameObject> lootTable;
    public EnemyStats enemyStats;
    [Header("HealthUI (Only For Player)", order = 2)]
    public Animator damageEffect;
    public GameObject healthBar;
    public TextMeshProUGUI maxHealthText;
    public GameObject gameOverUI;
    [Header("Shield (Only for player)", order = 3)]
    public GameObject shieldBar;
    public TextMeshProUGUI maxShieldText;
    public float shieldRegenTime;
    public float shieldRegenAmount;
    public bool inCombat;
    public float inCombatShieldRegenDelay;
    public float timeOutOfCombat;
    [Header("Props (Only for Props)", order = 4)]
    public GameObject propDestroyParticle;
    [Header("Explosive (Only for Explosive Props)", order = 5)]
    public float explosionRadius;
    public float explosionForce;
    public float exposionDamage;

    private void Start() {
        if (healthType == HealthType.Player) {
            UpdateHealthBar();
            maxShield = maxHealth / 4;
            shield = maxShield;
            UpdateShieldBar();
        }
        if(healthType == HealthType.Enemy) {
            health = enemyStats.health;
            maxHealth = enemyStats.health;
        }
    }
    // Update is called once per frame
    private void Update()
    {
        //makes sure the health is never negative
        if (health <= 0)
        {
            EnemyDeath();
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
        // Update the current time
        timeOutOfCombat += Time.deltaTime;
        
        // Check if it's time to regenerate the shield
        if (timeOutOfCombat >= shieldRegenTime && shield <= maxShield && inCombat == false && healthType == HealthType.Player) {
            timeOutOfCombat = 0f; // Reset the timer
            shield += shieldRegenAmount;
            UpdateShieldBar();
        }
    }
    //returns the current amount of health
    public float GetHealth()
    {
        return health;
    }

    //returns the current Maximum amount of health
    public float GetMaxHealth() {
        return maxHealth;
    }

    //this bool can be used to quickly check if the entity is alive
    public bool IsAlive()
    {
        if (health > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //damage the player
    public void Damage(float amount)
    {
        if (healthType == HealthType.Player) {
            if (amount >= shield) {
                amount -= shield;
                shield = 0;
                Debug.Log($"damage amount is {amount}");
                health -= amount;
            } else {
                shield -= amount;
            }
            UpdateShieldBar();
            UpdateHealthBar();
            StartCoroutine(InCombatShieldRegenDelay());
            if(health <= 0f) {
                PlayerDeath();
            }
            damageEffect.SetTrigger("isHit");
        }
        if (healthType == HealthType.Enemy || healthType == HealthType.prop) {
            health -= amount;
        }
    }

    //Clean way to heal player instead of dealing negative damage
    public void Heal(float amount)
    {
        if (health + amount > maxHealth) {
            health = maxHealth;
        } else {
            health += amount;
        }
        if (healthType == HealthType.Player) {
            UpdateHealthBar();
        }
    }

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
        if(healthType == HealthType.Player) {
            UpdateHealthBar();
        }
    }
    //Only for player:
    public void UpdateHealthBar() {
        healthBar.GetComponent<Slider>().value = health / maxHealth;
        maxHealthText.text = Mathf.Round(health).ToString();
    }
    public void UpdateShieldBar() {
        shieldBar.GetComponent<Slider>().value = shield / maxShield;
        maxShieldText.text = Mathf.Round(shield).ToString();
    }

    //returns the current amount of shield
    public float GetShield() {
        return shield;
    }

    //returns the current Maximum amount of shield
    public float GetMaxShield() {
        return maxShield;
    }
    private IEnumerator InCombatShieldRegenDelay() {
        inCombat = true;
        timeOutOfCombat = 0f;
        yield return new WaitForSeconds(inCombatShieldRegenDelay);
        inCombat = false;
    }
    public void EnemyDeath() {
        if (healthType == HealthType.Enemy || healthType == HealthType.prop) {
            Debug.Log("Enemy Died");
            if (healthType == HealthType.Enemy) {
                GameObject.Instantiate(deathSplash, transform.position, transform.rotation);
                if (Random.Range(0, 100) < 25 || enemyStats.name == "LootJalla") {
                    GameObject.Instantiate(lootTable.GetRandom(), transform.position, transform.rotation);
                }
                //gun.name = gunsToDropOnKill[randomIndex].name;
                if (GetComponent<Enemy>().enemyType != Enemy.EnemyTypes.lootJalla)
                {
                    GetComponent<Enemy>().spawner.OnEnemyKill();
                }
            }
            else if (healthType == HealthType.prop)
            {
                if (gameObject.CompareTag("Spawner")) {
                    GetComponent<Spawner>().generator.Teleporter().GetComponent<Teleporter>().spawnersLeft -= 1;
                }
                if (gameObject.CompareTag("Crate")) {
                    if (Random.Range(0, 100) < 15) {
                        GameObject.Instantiate(lootTable.GetRandom(), transform.position, transform.rotation);
                    }
                }
                if (gameObject.CompareTag("Explosive")) {
                    // Collect nearby colliders within the explosion radius
                    Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

                    foreach (Collider nearbyObject in colliders) {
                        // Apply explosion force to rigidbodies
                        if (nearbyObject.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
                            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                        }
                        if (nearbyObject.GetComponent<Health>()) {
                            nearbyObject.GetComponent<Health>().Damage(exposionDamage);
                        }
                    }
                }
                GameObject.Instantiate(propDestroyParticle, transform.position, transform.rotation);
            }
            Destroy(this.gameObject);
        }
    }
    public void PlayerDeath() {
        gameOverUI.SetActive(true);
        PauzeScript.gameIsPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
    }
}
