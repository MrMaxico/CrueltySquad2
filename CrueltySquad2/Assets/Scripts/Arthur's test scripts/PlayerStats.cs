using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int playerLevel;
    public float playerExp;
    public float nextLevelUpExp;
    public float playerMaxHealth;
    public Slider xpBar;
    public TextMeshProUGUI levelText;
    public Health healthcript;
    public float healthMultiplier = 1.2f;
    public float meleeDamage = 100f;
    public float meleedamageMultiplier = 1.2f;
    private void Start() {
        nextLevelUpExp = 50;
        UpdateXPBar();
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
        playerMaxHealth = playerMaxHealth * healthMultiplier;
        meleeDamage = meleeDamage * meleedamageMultiplier;
        healthcript.SetMaxHealth(playerMaxHealth);
        healthcript.Heal(playerMaxHealth);
        healthcript.shield = playerMaxHealth / 4;
        healthcript.updateShieldBar();
        nextLevelUpExp += 50;
        UpdateXPBar();
    }
    public void UpdateXPBar() {
        xpBar.value = playerExp / nextLevelUpExp;
        levelText.text = playerLevel.ToString();
    }
}
