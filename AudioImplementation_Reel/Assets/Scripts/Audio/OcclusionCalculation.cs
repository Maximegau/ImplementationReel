using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionCalculation : MonoBehaviour
{
    private AudioLowPassFilter lowPass;
    private AudioSource source;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        lowPass = GetComponent<AudioLowPassFilter>();
    }

    private void Update()
    {
        if (source.isPlaying)
        {
            AudioManager.Instance.CalculateOcclusionOnly(gameObject, lowPass, source);
        }
    }
}
