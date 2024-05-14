using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Analytics;
using UnityEngine.Advertisements;

public class Admob : MonoBehaviour
{
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
  private string _adUnitId = "unused";
#endif

    BannerView bannerView;

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

    public void Start()
    {
        MobileAds.Initialize(initStatus => 
        {
            Debug.Log("Google ads initialized");
            LoadAd();
        });
    }

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            DestroyBannerView();
        }
        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        
        // Create an anchored adaptive banner at bottom of the screen
        bannerView = new BannerView(_adUnitId, adaptiveSize, AdPosition.Bottom);
        ListenToAdEvents();
        
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);
    }
    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        if (bannerView != null) 
        {
            // Raised when an ad is loaded into the banner view.
            bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + bannerView.GetResponseInfo());
            };
            // Raised when an ad fails to load into the banner view.
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : "
                    + error);
            };
            // Raised when the ad is estimated to have earned money.
            bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            bannerView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Banner view full screen content closed.");
            };
        }
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            bannerView.Destroy();
            bannerView = null;
        }
    }

    private void OnGameStart(object userData)
    {
        if (Manager.Instance.tutorialEnabled)
        {

        }
        else
        {
            bannerView.Hide();
            //StartCoroutine(ShowAdWhenReady());
        }
    }

    private void OnEndGame(object userData)
    {
        if (Manager.Instance.tutorialEnabled)
        {

        }
        else
        {
            bannerView.Hide();
            //StartCoroutine(ShowAdWhenReady());
        }
    }
}