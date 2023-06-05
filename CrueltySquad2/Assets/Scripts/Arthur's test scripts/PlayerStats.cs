using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerLevel;
    public float playerExp;
    public float playerMaxHealth;
    public Health healthcript;
    private float healthModifier;
    private float shield;

    public void AddExp(float exp) {
        playerExp += exp;
        if(Mathf.FloorToInt(playerExp / 50) + 1 <= playerLevel) {
            LevelUp(Mathf.FloorToInt(playerExp / 50) + 1);
        }
    }
    public void LevelUp(int level) {
        playerLevel = level;
        healthModifier = 1.2f * level;
        playerMaxHealth *= healthModifier;
        healthcript.SetMaxHealth(playerMaxHealth);
    }
}
