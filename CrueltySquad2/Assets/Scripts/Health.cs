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
    public GameObject[] gunsToDropOnKill;
    public WeightedRandomList<GameObject> lootTable;
    [Header("HealthBar (Only For Player)", order = 2)]
    public GameObject healthBar;
    public TextMeshProUGUI maxHealthText;
    [Header("Shield (Only for player)", order = 3)]
    public GameObject shieldBar;
    public TextMeshProUGUI maxShieldText;
    public float shieldRegenTime;
    public float shieldRegenAmount;
    public bool inCombat;
    public float inCombatShieldRegenDelay;
    public float timeOutOfCombat;
    private void Start() {
        if (healthType == HealthType.Player) {
            UpdateHealthBar();
            maxShield = maxHealth / 4;
            shield = maxShield;
            UpdateShieldBar();
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
            if(amount >= shield) {
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
                GameObject gun = lootTable.GetRandom();
                if (Random.Range(0, 100) < 25) {
                    GameObject.Instantiate(gun, transform.position, transform.rotation);
                }
                Debug.Log(gun.name);
                //gun.name = gunsToDropOnKill[randomIndex].name;
                GetComponent<Enemy>().spawner.OnEnemyKill();
            }
            else if (healthType == HealthType.prop)
            {
                GetComponent<Spawner>().generator.Teleporter().GetComponent<Teleporter>().spawnersLeft -= 1;
            }
            Destroy(this.gameObject);
        }
    }
}
