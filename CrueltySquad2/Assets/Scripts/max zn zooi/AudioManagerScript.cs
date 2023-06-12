using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManagerScript instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.group;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

        }

    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }

    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }


    //deze line code hieronder moet je copy pasten als je een geluidje wilt afspelen
    //FindObjectOfType<AudioManagerScript>().Play("naamvangeluidhier");

    //en deze voor als je geluid of muziek wilt laten stoppen
    //FindObjectOfType<AudioManager>().StopPlaying("GameplayMusic");
}

