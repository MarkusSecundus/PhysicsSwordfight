using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPauseHelper : MonoBehaviour
{
    public AudioSource audioSrc;

    public void PauseOrUnpause()
    {
        if (audioSrc.isPlaying)
            audioSrc.Pause();
        else
        {
            audioSrc.UnPause();
            if (!audioSrc.isPlaying)
                audioSrc.Play();
        }
    }
}
