using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float health;

    // Update is called once per frame
    private void Update()
    {
        //makes sure the health is never negative
        if (health < 0)
        {
            health = 0;
        }
    }

    //return the current amount of health
    public float GetHealth()
    {
        return health;
    }

    //damage the player
    public void Damage(float amount)
    {
        health -= amount;
    }

    //Clean way to heal player instead of dealing negative damage
    public void Heal(float amount)
    {
        health += amount;
    }
}
