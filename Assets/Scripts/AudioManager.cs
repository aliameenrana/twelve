using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip victorySound;
    public AudioClip defeatSound;
    public AudioClip placeSound;

    private AudioSource aSource;

    private void Awake()
    {
        aSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnVictory, OnVictory);
        EventManager.StartListening(EventNames.OnDefeat, OnDefeat);
        EventManager.StartListening(EventNames.OnPlaceGeeti, OnPlaceGeeti);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnVictory, OnVictory);
        EventManager.StopListening(EventNames.OnDefeat, OnDefeat);
        EventManager.StopListening(EventNames.OnPlaceGeeti, OnPlaceGeeti);
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

    private void OnPlaceGeeti(object userData)
    {
        aSource.clip = placeSound;
        aSource.Play();
    }
}
