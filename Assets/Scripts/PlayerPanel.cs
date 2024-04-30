using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerPanel : MonoBehaviour
{
    public Player assignedPlayer;

    public Text playerStatus;
    public Text playerName;
    public Image playerIcon;

    private Sprite humanIcon;
    private Sprite computerIcon;

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnBeginTurn, OnBeginTurn);
        EventManager.StartListening(EventNames.OnSpawnGeetis, OnSpawnGeetis);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnBeginTurn, OnBeginTurn);
        EventManager.StopListening(EventNames.OnSpawnGeetis, OnSpawnGeetis);
    }

    private void OnBeginTurn(object userData)
    {
        UpdatePanel();
        GetMove();
    }

    private void OnSpawnGeetis(object userData)
    {
        playerIcon.enabled = false;
        humanIcon = Manager.Instance.humanIcon;
        computerIcon = Manager.Instance.computerIcon;
    }

    public void UpdatePanel()
    {
        playerStatus.text = assignedPlayer.playerTurn ? "Taking turn" : "Awaiting turn";
        playerName.text = assignedPlayer.playerName;
        playerIcon.sprite = assignedPlayer.playerType == PlayerType.Human ? humanIcon : computerIcon;
        playerIcon.enabled = true;
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