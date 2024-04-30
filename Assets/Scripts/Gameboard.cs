using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Gameboard : MonoBehaviour
{
    public static Gameboard Instance { get; set; }

    Manager manager;
    public Sprite redSprite;
    public Sprite yellowSprite;
    public GameObject geetiPrefab;
    public Transform geetiContainer;
    public Transform slotsContainer;

    private Slot[,] slots;
    private List<Geeti> geetis = new List<Geeti>();

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnSpawnGeetis, OnSpawnGeetis);
        EventManager.StartListening(EventNames.OnEndTurn, OnEndTurn);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnSpawnGeetis, OnSpawnGeetis);
        EventManager.StopListening(EventNames.OnEndTurn, OnEndTurn);
    }

    private void Awake()
    {
        manager = FindObjectOfType<Manager>();
        Instance = this;
    }

    private void OnSpawnGeetis(object userData)
    {
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        int currentSlotIndex = 0;
        Player currentPlayer = manager.player2;
        for (int i = 0; i < 12; i++)
        {
            GameObject newGeeti = Instantiate(geetiPrefab, geetiContainer);
            newGeeti.name = currentPlayer.playerType.ToString() + "Geeti";

            Slot slot = slotsContainer.GetChild(currentSlotIndex).GetComponent<Slot>();
            Geeti geeti = newGeeti.GetComponent<Geeti>();
            geeti.GetComponent<Image>().sprite = yellowSprite;
            slot.Assign(geeti);
            geeti.Assign(slot);
            geeti.player = currentPlayer;
            geetis.Add(geeti);
            currentSlotIndex++;
            yield return new WaitForSeconds(0.1f);
        }
        currentPlayer = manager.player1;
        currentSlotIndex++;
        for (int i = 0; i < 12; i++)
        {
            GameObject newGeeti = Instantiate(geetiPrefab, geetiContainer);
            newGeeti.name = currentPlayer.playerType.ToString() + "Geeti";
            Slot slot = slotsContainer.GetChild(currentSlotIndex).GetComponent<Slot>();
            Geeti geeti = newGeeti.GetComponent<Geeti>();
            geeti.GetComponent<Image>().sprite = redSprite;
            slot.Assign(geeti);
            geeti.Assign(slot);
            geeti.player = currentPlayer;
            geetis.Add(geeti);
            currentSlotIndex++;
            yield return new WaitForSeconds(0.1f);
        }
        EventManager.TriggerEvent(EventNames.OnGeetisSpawned, null);

        if (Manager.Instance.tutorialEnabled)
        {
            Manager.Instance.BeginTutorial();
        }
        else
        {
            BeginTurn(manager.player2);
        }
    }

    public Slot GetClosestSlot(Vector2 position)
    {
        Slot[] slots = slotsContainer.GetComponentsInChildren<Slot>();
        Slot closestSlot = slots[0];
        float currentSlotDistance = Vector2.Distance(closestSlot.transform.position, position);
        float shortestDistance = currentSlotDistance;

        for(int i = 0; i < slots.Length; i++)
        {
            currentSlotDistance = Vector2.Distance(slots[i].transform.position, position);
            if (currentSlotDistance < shortestDistance)
            {
                closestSlot = slots[i];
                shortestDistance = currentSlotDistance;
            }
        }

        return closestSlot;
    }

    public Geeti GetIntermediateGeeti(Slot sourceSlot, Slot destinationSlot)
    {
        int xDiff = ((int)sourceSlot.index.x - (int)destinationSlot.index.x);
        int yDiff = ((int)sourceSlot.index.y - (int)destinationSlot.index.y);

        int closerSlotX;
        int closerSlotY;

        closerSlotX = xDiff != 0 ? (int)sourceSlot.index.x + (xDiff / Mathf.Abs(xDiff)) * -1 : (int)sourceSlot.index.x;
        closerSlotY = yDiff != 0 ? (int)sourceSlot.index.y + (yDiff / Mathf.Abs(yDiff)) * -1 : (int)sourceSlot.index.y;

        string clX = closerSlotX.ToString();
        string clY = closerSlotY.ToString();

        Slot slot = sourceSlot.immediateSlots.Find(o => (int)o.index.x == closerSlotX && (int)o.index.y == closerSlotY);
        Geeti currentGeeti = slot.geeti;
        return currentGeeti;
    }

    public void BeginTurn(Player player)
    {
        player.playerTurn = true;
        EventManager.TriggerEvent(EventNames.OnBeginTurn, player);
    }

    public void RemoveGeeti(Geeti geeti)
    {
        int index = geetis.FindIndex(o => o.currentSlot.index == geeti.currentSlot.index);
        geetis.RemoveAt(index);
    }

    public List<Geeti> GetGeetis(PlayerType playerType)
    {
        return geetis.FindAll(o => o.player.playerType == playerType).ToList();
    }

    private void OnEndTurn(object userData)
    {
        Player playerEndingTurn = (Player)userData;

        if (playerEndingTurn == manager.player1) 
        {
            manager.player2.playerTurn = true;
            manager.player1.playerTurn = false;
            BeginTurn(manager.player2);
        }
        else
        {
            manager.player2.playerTurn = false;
            manager.player1.playerTurn = true;
            BeginTurn(manager.player1);
        }
    }

    public bool GameEndCheck()
    {
        bool player1Defeated = true;
        bool player2Defeated = true;

        for (int i = 0; i < geetis.Count; i++)
        {
            if (player2Defeated)
            {
                if (geetis[i].player == manager.player2)
                {
                    player2Defeated = false;
                }
            }
            if (player1Defeated)
            {
                if (geetis[i].player == manager.player1)
                {
                    player1Defeated = false;
                }
            }
        }

        if (player1Defeated)
        {
            manager.player1.Defeated();
            EventManager.TriggerEvent(EventNames.OnEndGame, manager.player2);
            return true;
        }
        if (player2Defeated)
        {
            manager.player2.Defeated();
            EventManager.TriggerEvent(EventNames.OnEndGame, manager.player1);
            return true;
        }

        return false;
    }

    public void TakeAITurn()
    {
        manager.player2.playerTurn = true;
        manager.player1.playerTurn = false;
        BeginTurn(manager.player2);
    }
}