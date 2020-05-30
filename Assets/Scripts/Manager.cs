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

    public bool tutorialEnabled = true;

    private Tutorial tutorialObject;
    private Gameboard gameboardObject;

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
        tutorialEnabled = PlayerPrefs.GetInt(Constants.TutorialComplete, 0) == 0;
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
                gameboardObject.BeginTurn(computerPlayer);
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