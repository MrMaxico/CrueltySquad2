using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    public AudioSource [] monsterAttack;


    void SoundAttack()
    {
        int index = Random.Range(0, monsterAttack.Length);
        monsterAttack[index].Play();
        monsterAttack[index].pitch = Random.Range(0.7f, 1.3f);
    }
}
