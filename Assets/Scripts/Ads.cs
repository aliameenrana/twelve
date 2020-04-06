using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour, IUnityAdsListener
{
    public bool isTestMode = true;
    public string videoAdID = "video";

    private const string gameID = "3538581";

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnGeetisSpawned, OnGameStart);
        EventManager.StartListening(EventNames.OnEndGame, OnEndGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnGeetisSpawned, OnGameStart);
        EventManager.StopListening(EventNames.OnEndGame, OnEndGame);
    }

    private void Start()
    {
        Advertisement.Initialize("3538581", isTestMode);
    }

    private void OnGameStart(object userData)
    {
        StartCoroutine(ShowAdWhenReady());
    }

    private void OnEndGame(object userData)
    {
        StartCoroutine(ShowAdWhenReady());
    }

    IEnumerator ShowAdWhenReady()
    {
        while (!Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }        
        Advertisement.Show();
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("OnUnityAdsReady: " + placementId);
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Unity Ads Error: " + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("OnUnityAdsDidStart: " + placementId);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        Debug.Log("OnUnityAdsDidFinish: " + placementId + "ShowResult: " + showResult.ToString());
    }
}