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
    [SerializeField] float maxShield;
    public float xpOnDeath = 10;
    [Header("Type of user", order = 0)]
    public HealthType healthType;
    [Header("Death (Only for Enemies)", order = 1)]
    public GameObject deathSplash;
    public GameObject[] gunsToDropOnKill;
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
            updateHealthBar();
            maxShield = maxHealth / 4;
            shield = maxShield;
            updateShieldBar();
        }
    }
    // Update is called once per frame
    private void Update()
    {
        //makes sure the health is never negative
        if (health <= 0)
        {
            if (healthType == HealthType.Enemy || healthType == HealthType.prop) {
                GameObject.Instantiate(deathSplash, transform.position, transform.rotation);
                int randomIndex = Random.Range(0, gunsToDropOnKill.Length);
                GameObject.Instantiate(gunsToDropOnKill[randomIndex]);
                Destroy(this.gameObject);
            }
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
        // Update the current time
        timeOutOfCombat += Time.deltaTime;
        
        // Check if it's time to regenerate the shield
        if (timeOutOfCombat >= shieldRegenTime && shield <= maxShield && inCombat == false) {
            timeOutOfCombat = 0f; // Reset the timer
            shield += shieldRegenAmount;
            updateShieldBar();
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
        if (healthType == HealthType.Player && shield != 0) {
            if(shield < amount) {
                amount -= shield;
                shield = 0;
                updateShieldBar();
            }
        }
        health -= amount;
        if (healthType == HealthType.Player) {
            updateHealthBar();
            StartCoroutine(InCombatShieldRegenDelay());
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
            updateHealthBar();
        }
    }

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
        if(healthType == HealthType.Player) {
            updateHealthBar();
        }
    }
    //Only for player:
    public void updateHealthBar() {
        healthBar.GetComponent<Slider>().value = health / maxHealth;
        maxHealthText.text = Mathf.Round(health).ToString();
    }
    public void updateShieldBar() {
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
}
