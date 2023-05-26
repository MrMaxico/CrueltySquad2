using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    public AudioSource [] monsterAttack;
    public AudioSource [] monsterAttack2;
    public AudioSource [] walking;



    void SoundAttack()
    {
        int index = Random.Range(0, monsterAttack.Length);
        monsterAttack[index].Play();
        monsterAttack[index].pitch = Random.Range(0.7f, 1.3f);
    }

    void SoundAttack2()
    {
        int index = Random.Range(0, monsterAttack.Length);
        monsterAttack2[index].Play();
        monsterAttack2[index].pitch = Random.Range(0.7f, 1.3f);
    }

    void SoundWalking()
    {
        int index = Random.Range(0, monsterAttack.Length);
        walking[index].Play();
        walking[index].pitch = Random.Range(0.5f, 0.8f);
    }
}
