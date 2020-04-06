using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerPanel : MonoBehaviour
{
    public Player assignedPlayer;

    public Text playerStatus;

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnBeginTurn, OnBeginTurn);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnBeginTurn, OnBeginTurn);
    }

    private void OnBeginTurn(object userData)
    {
        UpdatePanel();
        GetMove();
    }

    public void UpdatePanel()
    {
        playerStatus.text = assignedPlayer.playerTurn ? "Taking turn" : "Awaiting turn";
    }

    private void GetMove()
    {
        if (assignedPlayer.playerTurn)
        {
            if (assignedPlayer.playerType == PlayerType.Computer)
            {
                Dictionary<Geeti, Slot> calculatedMove = assignedPlayer.GetMove();
                if (calculatedMove != null)
                {
                    Geeti[] geetiArray = new Geeti[1];
                    calculatedMove.Keys.CopyTo(geetiArray, 0);
                    Geeti geeti = geetiArray[0];
                    Slot slot = calculatedMove[geeti];
                    geeti.transform.DOMove(slot.transform.position, 1.0f).OnComplete(() => geeti.OnEndAIDrag(geeti.transform.position));
                }
                else
                {
                    //End turn somehow
                    EventManager.TriggerEvent(EventNames.OnEndTurn, assignedPlayer);
                }
            }
        }
    }
}