using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu (menuName = "Audio/SFX/House")]
public class Houses : AudioEvent
{
    public List<AudioClip> clips = new List<AudioClip>();
    public int noRepeat = 2;
    public RangedFloat pitchRange;
    public RangedFloat volumeRange;
    public AudioMixerGroup bus;
    public bool coneAttenuation;

    public void Init()
    {
        if (noRepeat >= clips.Count) noRepeat = clips.Count - 1;

    }


    public override void Play(AudioSource source)
    {
        RandomizePitchAndVolume(source);
        source.spatialize = true;
        if (coneAttenuation)
        {

        }
        source.clip = PickAClip();
        source.Play();
    }

    private void RandomizePitchAndVolume(AudioSource source)
    {
        source.pitch = Random.Range(pitchRange.minValue, pitchRange.maxValue);
        source.volume = Random.Range(volumeRange.minValue, volumeRange.maxValue);
    }

    private AudioClip PickAClip()
    {
        if (clips.Count == 1)
        {
            return clips[0];
        }

        AudioClip clipToReturn = clips[Random.Range(0, clips.Count)];
        clips.Remove(clipToReturn);
        clips.Add(clipToReturn);
        return clipToReturn;
    }
}
