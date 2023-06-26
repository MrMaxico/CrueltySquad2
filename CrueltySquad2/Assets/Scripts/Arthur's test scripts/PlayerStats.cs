using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    static int playerLevel;
    static float playerExp;
    static float nextLevelUpExp;
    static float playerMaxHealth;
    static float maxShield;
    public Slider xpBar;
    public TextMeshProUGUI levelText;
    public Health healthcript;
    public float healthMultiplier = 1.2f;
    public float meleeDamage = 100f;
    public float meleedamageMultiplier = 1.2f;
    public Animator levelUP;

    private void Start() {
        if (playerLevel == 0)
        {
            playerLevel = 1;
            nextLevelUpExp = 50;
            playerMaxHealth = healthcript.GetMaxHealth();
            maxShield = healthcript.GetMaxShield();
        }
        healthcript.SetMaxHealth(playerMaxHealth);
        healthcript.Heal(playerMaxHealth);
        healthcript.maxShield = maxShield;
        healthcript.shield = maxShield;
        UpdateXPBar();
        healthcript.UpdateHealthBar();
        healthcript.UpdateShieldBar();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp(1);
        }
    }

    public void AddExp(float exp) {
        playerExp += exp;
        UpdateXPBar();
        if (playerExp >= nextLevelUpExp) {
            if (playerExp - nextLevelUpExp > 0) {
                float thisLevelUpXP = nextLevelUpExp;
                LevelUp(1);
                float extraExp = playerExp - thisLevelUpXP;
                playerExp = 0;
                AddExp(extraExp);
            } else {
                playerExp = 0f;
                LevelUp(1);
            }
        }
    }
    public void LevelUp(int level) {
        playerLevel += level;
        playerMaxHealth *= healthMultiplier;
        meleeDamage = meleeDamage * meleedamageMultiplier;
        healthcript.SetMaxHealth(playerMaxHealth);
        healthcript.Heal(playerMaxHealth);
        healthcript.shield = playerMaxHealth / 4;
        healthcript.maxShield = healthcript.shield;
        maxShield = healthcript.maxShield;
        healthcript.UpdateShieldBar();
        nextLevelUpExp += 50;
        UpdateXPBar();
        levelUP.SetTrigger("isLevelUp");
    }
    public void UpdateXPBar() {
        xpBar.value = playerExp / nextLevelUpExp;
        levelText.text = playerLevel.ToString();
    }
}
