using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXTracks
{
    BatDead,
    ButtonTriggered,
    DoorBroken,
    DoorOpen,
    DoorClose,
    GotSnowball,
    SpitSnowball,
    MonsterDead,
    ProjectileImpact
}

public class SFXBehavior : MonoBehaviour
{
    public AudioSource sfxAudioSource;
    
    public static SFXBehavior Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlaySFX(SFXTracks sfxName)
    {
        var sfx = Resources.Load<AudioClip>("Audio/SFX/SFXv0_" + sfxName);
        sfxAudioSource.PlayOneShot(sfx);
    }
}
