using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Geeti : MonoBehaviour
{
    public Slot currentSlot;
    public Player player;

    private Animation anim;
    private bool hasKilled = false;
    private bool hasChangedSlots = false;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
        if (!player.playerTurn)
        {
            CannotMove();
            return;
        }

        currentSlot.ShowValidSlots();
        EventManager.TriggerEvent(EventNames.OnBeginDrag, currentSlot);
    }

    public void OnDrag(BaseEventData eventData)
    {
        if (!player.playerTurn)
        {
            CannotMove();
            return;
        }

        transform.position = eventData.currentInputModule.input.mousePosition;
    }

    public void OnEndDrag(BaseEventData eventData)
    {
        if (!player.playerTurn)
        {
            CannotMove();
            return;
        }

        hasChangedSlots = false;

        Slot newSlot = Gameboard.Instance.GetClosestSlot(eventData.currentInputModule.input.mousePosition);
        if (currentSlot.IsValidSlot(newSlot))
        {
            hasKilled = false;
            hasChangedSlots = false;
            Geeti intermediateGeeti = Gameboard.Instance.GetIntermediateGeeti(currentSlot, newSlot);
            if (intermediateGeeti != null)
            {
                intermediateGeeti.Die();
                hasKilled = true;
            }
            currentSlot.UnAssign();
            newSlot.Assign(this);
            this.Assign(newSlot);
            hasChangedSlots = true;
            EventManager.TriggerEvent(EventNames.OnEndDrag, newSlot);
        }
        else
        {
            SnapToSlot();
            EventManager.TriggerEvent(EventNames.OnEndDrag, null);
        }
        OnEndMove();
        EventManager.TriggerEvent(EventNames.OnStopGlow, null);
    }

    public void OnEndAIDrag(Vector2 position)
    {
        Slot newSlot = Gameboard.Instance.GetClosestSlot(position);
        if (currentSlot.IsValidSlot(newSlot))
        {
            hasKilled = false;
            hasChangedSlots = false;

            Geeti intermediateGeeti = Gameboard.Instance.GetIntermediateGeeti(currentSlot, newSlot);
            if (intermediateGeeti != null)
            {
                intermediateGeeti.Die();
                hasKilled = true;
            }
            currentSlot.UnAssign();
            newSlot.Assign(this);
            this.Assign(newSlot);
            hasChangedSlots = true;
        }
        else
        {
            SnapToSlot();
        }
        OnEndMove();
        EventManager.TriggerEvent(EventNames.OnStopGlow, null);
    }

    private void SnapToSlot()
    {
        transform.position = currentSlot.transform.position;
    }

    public void Assign(Slot inSlot)
    {
        currentSlot = inSlot;
    }

    public void Die()
    {
        Gameboard.Instance.RemoveGeeti(this);
        currentSlot.UnAssign();
        //Destroy(gameObject);
        StartCoroutine(BlinkThenDestroy());
    }

    IEnumerator BlinkThenDestroy()
    {
        Image image = GetComponent<Image>();
        image.enabled = false;
        yield return new WaitForSeconds(0.2f);
        image.enabled = true;
        yield return new WaitForSeconds(0.2f);
        image.enabled = false;
        yield return new WaitForSeconds(0.2f);
        image.enabled = true;
        yield return new WaitForSeconds(0.2f);
        image.enabled = false;
        Destroy(gameObject);
    }

    public bool CanKill()
    {
        return (currentSlot.GetKillableSlots().Count > 0);
    }

    public bool CanMove()
    {
        return (currentSlot.GetEmptyValidSlots().Count > 0);
    }

    private void CannotMove()
    {
        
    }

    public void OnEndMove()
    {
        if (hasChangedSlots)
        {
            if (hasKilled)
            {
                if (Gameboard.Instance.GameEndCheck())
                {

                }
                else
                {
                    //Has one more move. Can either end turn or not. Show prompt
                    if (this.player.playerType == PlayerType.Computer)
                    {
                        Action action = new Action(() => Gameboard.Instance.TakeAITurn());
                        StartCoroutine(Utilities.Run(action, 1.5f));
                    }
                }
            }
            else
            {
                //End Turn automatically
                EventManager.TriggerEvent(EventNames.OnEndTurn, this.player);
            }
        }
        else
        {
            //Cannot End Turn
        }
    }

    public Slot GetBestKillingSlot()
    {
        List<Slot> killingSlots = currentSlot.GetKillableSlots();
        return killingSlots[UnityEngine.Random.Range(0, killingSlots.Count)];
    }

    public Slot GetBestMovingSlot()
    {
        List<Slot> movingSlots = currentSlot.GetEmptyValidSlots();
        return movingSlots[UnityEngine.Random.Range(0, movingSlots.Count)];
    }

    public List<Slot> GetSafeMovingSlots()
    {
        List<Slot> movingSlots = currentSlot.GetEmptyValidSlots();
        List<Slot> safeMovingSlots = new List<Slot>();
        safeMovingSlots = movingSlots.FindAll(o => o.IsSafe(currentSlot, o, Manager.Instance.computerPlayer));
        return safeMovingSlots;
    }
}