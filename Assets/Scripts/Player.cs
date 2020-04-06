using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PlayerType
{
    Human,
    Computer
}

public class Player
{
    public PlayerType playerType;
    public bool playerTurn = false;

    public Dictionary<Geeti, Slot> GetMove()
    {
        List<Geeti> geetis = Gameboard.Instance.GetGeetis(playerType);
        List<Geeti> killerGeetis = geetis.FindAll(o => o.CanKill()).ToList();
        List<Geeti> moveableGeetis = geetis.FindAll(o => o.CanMove()).ToList();

        if (killerGeetis.Count > 0)
        {
            int killerGeetiIndex = Random.Range(0, killerGeetis.Count);
            Geeti geetiToReturn = killerGeetis[killerGeetiIndex];
            Slot slotToReturn = geetiToReturn.GetBestKillingSlot();
            Dictionary<Geeti, Slot> dict = new Dictionary<Geeti, Slot>();
            dict.Add(geetiToReturn, slotToReturn);
            return dict;
        }
        else
        {
            if (moveableGeetis.Count > 0)
            {
                Geeti geetiToReturn;
                Slot slotToReturn;

                Dictionary<Geeti, List<Slot>> safelyMoveableGeetis = new Dictionary<Geeti, List<Slot>>();

                for (int i = 0; i < moveableGeetis.Count; i++)
                {
                    List<Slot> safeSlots = moveableGeetis[i].GetSafeMovingSlots();
                    if (safeSlots.Count > 0)
                    {
                        safelyMoveableGeetis.Add(moveableGeetis[i], safeSlots);
                    }
                }

                if (safelyMoveableGeetis.Count > 0)
                {
                    var randomKey = safelyMoveableGeetis.Keys.ToArray()[(int)Random.Range(0, safelyMoveableGeetis.Keys.Count - 1)];
                    var randomValueFromDictionary = safelyMoveableGeetis[randomKey];

                    geetiToReturn = randomKey;
                    slotToReturn = randomValueFromDictionary[Random.Range(0, randomValueFromDictionary.Count)];
                }
                else
                {
                    int moveableGeetiIndex = Random.Range(0, moveableGeetis.Count);
                    geetiToReturn = moveableGeetis[moveableGeetiIndex];
                    slotToReturn = geetiToReturn.GetBestMovingSlot();
                }

                
                Dictionary<Geeti, Slot> dict = new Dictionary<Geeti, Slot>();
                dict.Add(geetiToReturn, slotToReturn);
                return dict;
            }
            else
            {
                return null;
            }
        }
    }

    public void Defeated()
    {
        
    }
}