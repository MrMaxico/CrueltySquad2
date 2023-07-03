using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public string name;
    public float damage;
    public float health;

    public void Start() {
        for (int i = 0; i < Teleporter.islandNumber - 1; i++) {
            damage += 0.3f * damage;
            health += 0.3f * health;
        }
    }
}
