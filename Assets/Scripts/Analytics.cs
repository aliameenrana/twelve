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
        if (Manager.Instance.player1.playerType!= Manager.Instance.player2.playerType) 
        {
            AnalyticsEvent.Custom("victory");
        }
    }

    private void OnDefeat(object userData)
    {
        if (Manager.Instance.player1.playerType != Manager.Instance.player2.playerType)
        {
            AnalyticsEvent.Custom("defeat");
        }
    }
}