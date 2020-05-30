using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum TutorialStage
{
    PawnsIntro,
    EmptySlotIntro,
    FirstComputerCompleteMove,
    HumanMoveBegin,
    FirstHumanKillComplete,
    TutorialComplete
}

public class Tutorial : MonoBehaviour
{
    public TutorialStage tutorialStage;
    public Transform TutorialCanvas;
    public GameObject nextButton;

    public void BeginTutorial()
    {
        TutorialCanvas.gameObject.SetActive(true);
        tutorialStage = TutorialStage.PawnsIntro;
        TutorialCanvas.GetChild((int)tutorialStage).gameObject.SetActive(true);
    }

    public void OnClickNext()
    {
        switch(tutorialStage)
        {
            case TutorialStage.PawnsIntro:
                TutorialCanvas.GetChild(0).gameObject.SetActive(false);
                TutorialCanvas.GetChild(1).gameObject.SetActive(true);
                tutorialStage++;
                break;
            case TutorialStage.EmptySlotIntro:
                TutorialCanvas.GetChild(1).gameObject.SetActive(false);
                TutorialCanvas.GetChild(2).gameObject.SetActive(true);
                tutorialStage++;
                break;
            case TutorialStage.FirstComputerCompleteMove:
                Manager.Instance.TutorialRequest(TutorialStage.FirstComputerCompleteMove);
                TutorialCanvas.GetChild(2).gameObject.SetActive(false);
                nextButton.SetActive(false);
                Action action = new Action(() => TutorialCanvas.GetChild(3).gameObject.SetActive(true));
                StartCoroutine(Run(action, 1.5f));
                tutorialStage++;
                break;
            case TutorialStage.HumanMoveBegin:
                TutorialCanvas.GetChild(3).gameObject.SetActive(false);
                Action action2 = new Action(() => TutorialCanvas.GetChild(4).gameObject.SetActive(true));
                StartCoroutine(Run(action2, 1.5f));
                tutorialStage++;
                break;
            case TutorialStage.FirstHumanKillComplete:
                PlayerPrefs.SetInt(Constants.TutorialComplete, 1);
                Manager.Instance.tutorialEnabled = false;
                TutorialCanvas.GetChild(5).gameObject.SetActive(true);
                nextButton.SetActive(true);
                tutorialStage++;
                break;
            case TutorialStage.TutorialComplete:
                TutorialCanvas.GetChild(5).gameObject.SetActive(false);
                nextButton.SetActive(false);
                break;
        }
    }

    IEnumerator Run(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public void BeginDrag()
    {
        if (tutorialStage == TutorialStage.HumanMoveBegin)
        {
            TutorialCanvas.GetChild(3).gameObject.SetActive(false);
        }
        else if (tutorialStage == TutorialStage.FirstHumanKillComplete)
        {
            TutorialCanvas.GetChild(4).gameObject.SetActive(false);
        }
    }

    public void EndDragUnsuccessful()
    {
        if (tutorialStage == TutorialStage.HumanMoveBegin)
        {
            TutorialCanvas.GetChild(3).gameObject.SetActive(true);
        }
        else if (tutorialStage == TutorialStage.FirstHumanKillComplete)
        {
            TutorialCanvas.GetChild(4).gameObject.SetActive(true);
        }
    }
}