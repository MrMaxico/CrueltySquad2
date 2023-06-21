using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    public AudioSource [] monsterAttack;
    public AudioSource [] monsterAttack2;
    public AudioSource [] walking;
    public AudioSource [] normalsound;



    void SoundAttack()
    {
        int index = Random.Range(0, monsterAttack.Length - 1);
        monsterAttack[index].Play();
        monsterAttack[index].pitch = Random.Range(0.7f, 1.3f);
    }

    void SoundAttack2()
    {
        int index = Random.Range(0, monsterAttack2.Length - 1);
        monsterAttack2[index].Play();
        monsterAttack2[index].pitch = Random.Range(0.7f, 1.3f);
    }

    void SoundWalking()
    {
        int index = Random.Range(0, walking.Length - 1);
        walking[index].Play();
        walking[index].pitch = Random.Range(0.5f, 0.8f);
    }

    public void NormalPitchSound()
    {
        int index = Random.Range(0, normalsound.Length - 1);
        normalsound[index].Play();
    }
}
