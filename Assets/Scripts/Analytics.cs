using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analytics : MonoBehaviour
{
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
        AnalyticsEvent.Custom("victory");
    }

    private void OnDefeat(object userData)
    {
        AnalyticsEvent.Custom("defeat");
    }
}