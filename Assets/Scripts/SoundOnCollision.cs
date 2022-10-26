using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    public string TargetTag;

    public AudioClip[] sounds;

    private AudioSource audioSrc;


    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (string.IsNullOrEmpty(TargetTag) || collision.gameObject.CompareTag(TargetTag))
            audioSrc.StartPlaying(sounds.RandomElement(), forceAnew: false);
    }
}
