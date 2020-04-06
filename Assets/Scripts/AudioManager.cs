using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip victorySound;
    public AudioClip defeatSound;

    private AudioSource aSource;

    private void Awake()
    {
        aSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnVictory, OnVictory);
        EventManager.StartListening(EventNames.OnDefeat, OnDefeat);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnVictory, OnVictory);
        EventManager.StopListening(EventNames.OnDefeat, OnDefeat);
    }

    private void OnVictory(object userData)
    {
        aSource.clip = victorySound;
        aSource.Play();
    }

    private void OnDefeat(object userData)
    {
        aSource.clip = defeatSound;
        aSource.Play();
    }
}
