using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Audio/SFX/Ambiance")]
public class Ambiance : AudioEvent
{
    public AudioClip ambiance;


    public override void Play(AudioSource source)
    {
        if (!source.isPlaying)
        {
            source.Play();
        }
    }
}
