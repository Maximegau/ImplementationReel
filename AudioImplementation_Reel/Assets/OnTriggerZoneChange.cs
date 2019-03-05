using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerZoneChange : MonoBehaviour
{
    public AudioManager.location switchToLocation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (switchToLocation)
            {
                case (AudioManager.location.village):
                    AudioManager.Instance.CurrentLocation_Village();
                    break;
                case (AudioManager.location.forest):
                    AudioManager.Instance.CurrentLocation_Forest();
                    break;
            }
            
        }
    }
}
