﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source;
    public static AudioManager Instance;
    public GameObject audioSourcePrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySoundAtPoint(AudioClip clip, GameObject sourceObject)
    {
        AudioSource.PlayClipAtPoint(clip, sourceObject.transform.position);
    }

    public void PlaySoundClip(AudioClip clip, float volume)
    {
        source.PlayOneShot(clip, volume);
    }
}
