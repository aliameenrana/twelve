using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; set; }
    [System.NonSerialized]
    public Player computerPlayer;
    [System.NonSerialized]
    public Player humanPlayer;

    public PlayerPanel humanPanel;
    public PlayerPanel computerPanel;

    public GameObject MainMenu;
    public GameObject Gameboard;
    public GameObject EndGameMenu;
    public GameObject VictoryText;
    public GameObject GameOverText;

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnEndGame, OnEndGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnEndGame, OnEndGame);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void OnGameSetup()
    {
        computerPlayer = CreatePlayer(PlayerType.Computer, computerPanel);
        humanPlayer = CreatePlayer(PlayerType.Human, humanPanel);
        MainMenu.SetActive(false);
        Gameboard.SetActive(true);
        EventManager.TriggerEvent(EventNames.OnSpawnGeetis, null);
    }

    private Player CreatePlayer(PlayerType type, PlayerPanel panel)
    {
        Player player = new Player();
        player.playerType = type;
        panel.assignedPlayer = player;
        return player;
    }

    private void OnEndGame(object userData)
    {
        bool win = ((Player)userData).playerType == PlayerType.Human;
        VictoryText.SetActive(win);
        GameOverText.SetActive(!win);
        EndGameMenu.SetActive(true);
        if (win)
        {
            EventManager.TriggerEvent(EventNames.OnVictory, null);
        }
        else
        {
            EventManager.TriggerEvent(EventNames.OnDefeat, null);
        }
    }

    public void OnPlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}