using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum HealthType {
    Enemy,
    Player,
    prop,
    // Add more gun types as needed
}
public class Health : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    [SerializeField] float shield;
    [SerializeField] float maxShield;
    public GameObject healthBar;
    public GameObject shieldBar;
    public TextMeshProUGUI maxHealthText;
    public HealthType healthType;
    private void Start() {
        if (healthType == HealthType.Player) {
            updateHealthBar();
            maxShield = maxHealth / 4;
            updateShieldBar();
        }
    }
    // Update is called once per frame
    private void Update()
    {
        //makes sure the health is never negative
        if (health < 0)
        {
            health = 0;
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
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
            }
        }
        health -= amount;
        if (healthType == HealthType.Player) {
            updateHealthBar();
        }
    }

    //Clean way to heal player instead of dealing negative damage
    public void Heal(float amount)
    {
        health += amount;
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
        maxHealthText.text = health.ToString();
    }
    public void updateShieldBar() {
        shieldBar.GetComponent<Slider>().value = shield / maxShield;
        maxHealthText.text = shield.ToString();
    }
}
