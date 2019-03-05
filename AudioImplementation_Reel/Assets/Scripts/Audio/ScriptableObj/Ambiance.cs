using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu (menuName = "Audio/SFX/Ambiance")]
public class Ambiance : AudioEvent
{
    public AudioClip ambiance;
    public AudioMixerGroup bus;


    public override void Play(AudioSource source)
    {
        if (!source.isPlaying)
        {
            source.clip = ambiance;
            source.volume = 1;
            source.pitch = 1;
            source.loop = true;
            source.outputAudioMixerGroup = bus;
            source.Play();
        }
    }

}
