using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    public AudioSource monsterAttack;
    
    void SoundAttack()
    {
        monsterAttack.Play();
        monsterAttack.pitch = Random.Range(0.7f, 1.3f);
    }
}
