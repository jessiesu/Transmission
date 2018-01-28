using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PhaseMusic : MonoBehaviour
{
    public AudioClip redAudio;
    public AudioClip blueAudio;
    private AudioSource audioSource;

    public float currentTime = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRedTrack()
    {
        PlayTrack(redAudio);
    }

    public void PlayBlueTrack()
    {
        PlayTrack(blueAudio);
    }

    public void PlayTrack(AudioClip track)
    {
        if (audioSource.isPlaying)
        {
            currentTime = audioSource.time;
        }

        audioSource.clip = track;
        audioSource.time = currentTime;
        audioSource.Play();
    }
}