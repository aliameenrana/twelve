using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Slot : MonoBehaviour
{
    public Geeti geeti;
    public Vector2 index;
    public List<Slot> immediateSlots = new List<Slot>();
    public List<Slot> oneStepLaterSlots = new List<Slot>();
    public List<Slot> validSlots = new List<Slot>();
    public bool omniDirectional;

    private Glow glow;

    private void Start()
    {
        glow = GetComponentInChildren<Glow>();
        geeti = null;
        GetSlots(1, immediateSlots);
        GetSlots(2, oneStepLaterSlots);
        GetValidSlots();
    }

    public void ShowValidSlots()
    {
        List<Slot> allSlots = transform.parent.GetComponentsInChildren<Slot>().ToList();
        foreach (var item in allSlots)
        {
            item.GetComponent<Image>().color = Color.white;
        }

        foreach (var item in immediateSlots)
        {
            if (item.IsEmptySlot())
            {
                //item.GetComponent<Image>().color = Color.green;
                item.BeginGlow(GlowColor.Green);
            }
        }

        foreach (var item in oneStepLaterSlots)
        {
            if (item.IsEmptySlot())
            {
                if (IsCuttingSlot(item))
                {
                    //item.GetComponent<Image>().color = Color.red;
                    item.BeginGlow(GlowColor.Red);
                }
            }
        }
    }

    private void GetSlots(int distance, List<Slot> saveIn)
    {
        List<Slot> allSlots = transform.parent.GetComponentsInChildren<Slot>().ToList();
        foreach (var item in allSlots)
        {
            if (Mathf.Abs(index.x - item.index.x) == distance && index.y == item.index.y)
            {
                saveIn.Add(item);
            }
            if (Mathf.Abs(index.y - item.index.y) == distance && index.x == item.index.x)
            {
                saveIn.Add(item);
            }
            if (omniDirectional)
            {
                if (Mathf.Abs(index.x - item.index.x) == distance && Mathf.Abs(index.y - item.index.y) == distance)
                {
                    saveIn.Add(item);
                }
            }
        }
    }

    private void GetValidSlots()
    {
        GetSlots(1, validSlots);
        GetSlots(2, validSlots);
    }

    public bool IsValidSlot(Slot slotToCheck)
    {
        if (slotToCheck.IsEmptySlot())
        {
            if (validSlots.Contains(slotToCheck))
            {
                if (immediateSlots.Contains(slotToCheck))
                {
                    return true;
                }
                else if (oneStepLaterSlots.Contains(slotToCheck))
                {
                    int xDiff = ((int)this.index.x - (int)slotToCheck.index.x);
                    int yDiff = ((int)this.index.y - (int)slotToCheck.index.y);

                    int closerSlotX;
                    int closerSlotY;

                    closerSlotX = xDiff != 0 ? (int)this.index.x + (xDiff / Mathf.Abs(xDiff)) * -1 : (int)this.index.x;
                    closerSlotY = yDiff != 0 ? (int)this.index.y + (yDiff / Mathf.Abs(yDiff)) * -1 : (int)this.index.y;

                    string clX = closerSlotX.ToString();
                    string clY = closerSlotY.ToString();

                    Slot slot = immediateSlots.Find(o => (int)o.index.x == closerSlotX && (int)o.index.y == closerSlotY);
                    Geeti currentGeeti = slot.geeti;
                    if (currentGeeti != null)
                    {
                        if (currentGeeti.player != geeti.player)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Can I, the calling slot cut a geeti and go to SlotToCheck?
    /// </summary>
    /// <param name="slotToCheck">The empty slot where I can land after cutting a geeti</param>
    /// <returns></returns>
    public bool IsCuttingSlot(Slot slotToCheck, Player player = null)
    {
        int xDiff = ((int)this.index.x - (int)slotToCheck.index.x);
        int yDiff = ((int)this.index.y - (int)slotToCheck.index.y);

        int closerSlotX;
        int closerSlotY;

        closerSlotX = xDiff != 0 ? (int)this.index.x + (xDiff / Mathf.Abs(xDiff)) * -1 : (int)this.index.x;
        closerSlotY = yDiff != 0 ? (int)this.index.y + (yDiff / Mathf.Abs(yDiff)) * -1 : (int)this.index.y;

        Slot slot = immediateSlots.Find(o => (int)o.index.x == closerSlotX && (int)o.index.y == closerSlotY);
        if (slot != null)
        {
            Geeti currentGeeti = slot.geeti;
            if (currentGeeti != null)
            {
                if (currentGeeti.player != geeti.player)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (player != null)
                {
                    if (this.geeti.player.playerType != player.playerType)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }

    public bool IsEmptySlot()
    {
        return geeti == null;
    }

    public void Assign(Geeti inGeeti)
    {
        geeti = inGeeti;
        geeti.transform.position = transform.position;
    }

    public void UnAssign()
    {
        geeti = null;
    }

    public void BeginGlow(GlowColor glowColor)
    {
        glow.BeginGlow(glowColor);
    }

    public List<Slot> GetKillableSlots()
    {
        List<Slot> KillableSlots = new List<Slot>();
        foreach (var item in oneStepLaterSlots)
        {
            if (item.IsEmptySlot())
            {
                if (IsCuttingSlot(item))
                {
                    KillableSlots.Add(item);
                }
            }
        }
        return KillableSlots;
    }

    public List<Slot> GetEmptyValidSlots()
    {
        List<Slot> emptyValidSlots = new List<Slot>();
        foreach (var item in immediateSlots)
        {
            if (item.IsEmptySlot())
            {
                emptyValidSlots.Add(item);
            }
        }
        return emptyValidSlots;
    }

    public List<Slot> GetEmptySafeSlots()
    {
        List<Slot> emptyValidSlots = new List<Slot>();
        foreach (var item in immediateSlots)
        {
            if (item.IsEmptySlot())
            {
                emptyValidSlots.Add(item);
            }
        }
        return emptyValidSlots;
    }
    
    public bool IsSafe(Slot sourceSlot, Slot destinationSlot, Player player)
    {
        List<Slot> enemySlotsInImmediateVicinity = GetEnemySlotsInImmediateVicinity(destinationSlot, player);
        List<Slot> emptySlotsInImmediateVicinity = GetEmptySlotsInImmediateVicinity(destinationSlot);

        bool safe = true;

        for (int i = 0; i < enemySlotsInImmediateVicinity.Count; i++)
        {
            Slot slotToCheckAgainst = enemySlotsInImmediateVicinity[i];

            for(int j = 0; j < emptySlotsInImmediateVicinity.Count; j++)
            {
                if (slotToCheckAgainst.IsCuttingSlot(emptySlotsInImmediateVicinity[j]))
                {
                    safe = false;
                }
            }
            if (slotToCheckAgainst.IsCuttingSlot(sourceSlot, player))
            {
                safe = false;
            }
        }
        return safe;
    }

    private List<Slot> GetEnemySlotsInImmediateVicinity(Slot destinationSlot, Player player)
    {
        List<Slot> enemySlotsInImmediateVicinity = new List<Slot>();
        for(int i = 0; i < destinationSlot.immediateSlots.Count; i++)
        {
            if (destinationSlot.immediateSlots[i].geeti != null)
            {
                if (destinationSlot.immediateSlots[i].geeti.player.playerType != player.playerType)
                {
                    enemySlotsInImmediateVicinity.Add(destinationSlot.immediateSlots[i]);
                }
            }
        }
        return enemySlotsInImmediateVicinity;
    }

    private List<Slot> GetEmptySlotsInImmediateVicinity(Slot destinationSlot)
    {
        List<Slot> emptySlotsInImmediateVicinity = new List<Slot>();
        for (int i = 0; i < destinationSlot.immediateSlots.Count; i++)
        {
            if (destinationSlot.immediateSlots[i].IsEmptySlot())
            {
                emptySlotsInImmediateVicinity.Add(destinationSlot.immediateSlots[i]);
            }
        }
        return emptySlotsInImmediateVicinity;
    }
}