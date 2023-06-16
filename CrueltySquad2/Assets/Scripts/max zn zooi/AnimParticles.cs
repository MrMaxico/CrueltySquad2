using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimParticles : MonoBehaviour
{
    public ParticleSystem  monsterAttack;
    public ParticleSystem  monsterAttack2;
    // public AudioSource  walking;
    // public AudioSource  normalsound;

    private void Start()
    {
        //monsterAttack = GetComponent<ParticleSystem>();
       // monsterAttack2 = GetComponent<ParticleSystem>();
    }

    void PlayParticle1()
    {
        monsterAttack.Play();
    }

    void PlayParticle2()
    {
        monsterAttack2.Play();
    }

}
