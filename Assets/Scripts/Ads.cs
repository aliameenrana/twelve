using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public bool isTestMode = true;
    public string videoAdID = "video";
    public string bannerAdID = "bottomBanner";

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
        Advertisement.Initialize("3538581", isTestMode, this);

        StartCoroutine(ShowBannerWhenReady());
    }

    private void OnGameStart(object userData)
    {
        if (Manager.Instance.tutorialEnabled)
        {

        }
        else
        {
            Advertisement.Banner.Hide();
            StartCoroutine(ShowAdWhenReady());
        }
    }

    private void OnEndGame(object userData)
    {
        if (Manager.Instance.tutorialEnabled)
        {

        }
        else
        {
            Advertisement.Banner.Hide();
            StartCoroutine(ShowAdWhenReady());
        }
    }

    IEnumerator ShowAdWhenReady()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Load(videoAdID, this);
        Advertisement.Show(videoAdID, this);
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(bannerAdID);
        Advertisement.Banner.Show(bannerAdID);
    }

    void IUnityAdsLoadListener.OnUnityAdsAdLoaded(string placementId)
    {
        
    }

    void IUnityAdsLoadListener.OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        
    }

    void IUnityAdsInitializationListener.OnInitializationComplete()
    {
        Debug.Log("Ads initialization complete");
    }

    void IUnityAdsInitializationListener.OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        
    }

    void IUnityAdsShowListener.OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        
    }

    void IUnityAdsShowListener.OnUnityAdsShowStart(string placementId)
    {
        
    }

    void IUnityAdsShowListener.OnUnityAdsShowClick(string placementId)
    {
        
    }

    void IUnityAdsShowListener.OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        
    }
}