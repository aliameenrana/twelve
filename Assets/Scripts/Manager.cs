using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; set; }
    /// <summary>
    /// Human in a player vs computer game
    /// </summary>
    [System.NonSerialized]
    public Player player1;
    /// <summary>
    /// Computer in a player vs computer game
    /// </summary>
    [System.NonSerialized]
    public Player player2;

    public PlayerPanel player1Panel;
    public PlayerPanel player2Panel;

    public GameObject MainMenu;
    public GameObject Gameboard;
    public GameObject EndGameMenu;
    public GameObject VictoryText;
    public GameObject GameOverText;
    public GameObject Player1VictoryText;
    public GameObject Player2VictoryText;
    public GameObject stats;

    public bool tutorialEnabled = true;

    private Tutorial tutorialObject;
    private Gameboard gameboardObject;

    public Text numberOfVictories;
    public Text numberOfDefeats;

    public Sprite computerIcon;
    public Sprite humanIcon;

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnEndGame, OnEndGame);
        EventManager.StartListening(EventNames.OnBeginDrag, OnBeginDrag);
        EventManager.StartListening(EventNames.OnEndDrag, OnEndDrag);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnEndGame, OnEndGame);
        EventManager.StopListening(EventNames.OnBeginDrag, OnBeginDrag);
        EventManager.StopListening(EventNames.OnEndDrag, OnEndDrag);
    }

    private void Awake()
    {
        Instance = this;
        tutorialObject = FindObjectOfType<Tutorial>();
        gameboardObject = FindObjectOfType<Gameboard>();
        //tutorialEnabled = PlayerPrefs.GetInt(Constants.TutorialComplete, 0) == 0;
        UpdateStats();
    }

    private void UpdateStats()
    {
        int wins = PlayerPrefs.GetInt(Constants.Victories, 0);
        int losses = PlayerPrefs.GetInt(Constants.Defeats, 0);

        stats.SetActive(!(wins == 0 && losses == 0));
        numberOfVictories.text = wins.ToString();
        numberOfDefeats.text = losses.ToString();
    }

    public void OnGameSetup()
    {
        player2 = CreatePlayer(PlayerType.Computer, player2Panel, "Computer");
        player1 = CreatePlayer(PlayerType.Human, player1Panel, "Human");
        MainMenu.SetActive(false);
        Gameboard.SetActive(true);
        EventManager.TriggerEvent(EventNames.OnSpawnGeetis, null);
    }

    public void OnGameSetupHumanAgainstHuman()
    {
        player2 = CreatePlayer(PlayerType.Human, player2Panel, "Player 2");
        player1 = CreatePlayer(PlayerType.Human, player1Panel, "Player 1");
        MainMenu.SetActive(false);
        Gameboard.SetActive(true);
        EventManager.TriggerEvent(EventNames.OnSpawnGeetis, null);
    }

    private Player CreatePlayer(PlayerType type, PlayerPanel panel, string name)
    {
        Player player = new Player();
        player.playerName = name;
        player.playerType = type;
        panel.assignedPlayer = player;
        return player;
    }

    private void OnEndGame(object userData)
    {
        VictoryText.SetActive(false);
        GameOverText.SetActive(false);
        Player1VictoryText.SetActive(false);
        Player2VictoryText.SetActive(false);

        if (Manager.Instance.player1.playerType != Manager.Instance.player2.playerType) 
        {
            bool win = ((Player)userData).playerType == PlayerType.Human;
            VictoryText.SetActive(win);
            GameOverText.SetActive(!win);
            if (win)
            {
                int victories = PlayerPrefs.GetInt(Constants.Victories, 0);
                victories++;
                PlayerPrefs.SetInt(Constants.Victories, victories);
                EventManager.TriggerEvent(EventNames.OnVictory, null);
            }
            else
            {
                int losses = PlayerPrefs.GetInt(Constants.Defeats, 0);
                losses++;
                PlayerPrefs.SetInt(Constants.Defeats, losses);
                EventManager.TriggerEvent(EventNames.OnDefeat, null);
            }
        }
        else
        {
            bool player1Wins = ((Player)userData) == Manager.Instance.player1;
            Player1VictoryText.SetActive(player1Wins);
            Player2VictoryText.SetActive(!player1Wins);

            EventManager.TriggerEvent(EventNames.OnVictory, null);
        }
        EndGameMenu.SetActive(true);
    }

    public void BeginTutorial()
    {
        tutorialObject.BeginTutorial();
    }

    public void OnPlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    #region Tutorial
    private Slot slotBeforeDragging;

    public void TutorialRequest(TutorialStage tutorialStage)
    {
        switch (tutorialStage)
        {
            case TutorialStage.PawnsIntro:
                break;
            case TutorialStage.EmptySlotIntro:
                break;
            case TutorialStage.FirstComputerCompleteMove:
                gameboardObject.BeginTurn(player2);
                break;
            case TutorialStage.HumanMoveBegin:
                break;
            case TutorialStage.FirstHumanKillComplete:
                break;
            case TutorialStage.TutorialComplete:
                break;
        }
    }

    private void OnBeginDrag(object userData)
    {
        if (tutorialEnabled)
        {
            slotBeforeDragging = (Slot)userData;
            tutorialObject.BeginDrag();
        }
    }

    private void OnEndDrag(object userData)
    {
        if (tutorialEnabled)
        {
            if (userData != null)
            {
                Slot slotAfterDragging = (Slot)userData;
                tutorialObject.OnClickNext();
            }
            else
            {
                tutorialObject.EndDragUnsuccessful();
            }
        }
    }
    #endregion Tutorial
}