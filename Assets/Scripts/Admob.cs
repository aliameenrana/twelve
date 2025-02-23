using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class Admob : MonoBehaviour
{
    public bool isTesting = true;
    private static Admob instance;
    public string bannerViewID_Test = "ca-app-pub-3940256099942544/9214589741";
    public string interstitialID_Test = "ca-app-pub-3940256099942544/1033173712";
    public string bannerViewID_Live = "ca-app-pub-8838369119900775/1013070473";
    public string interstitialID_Live = "ca-app-pub-8838369119900775/4617273566";
    private string bannerViewID = "";
    private string interstitialAdID = ""; 

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

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

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
            InitializeAds();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }

        #if UNITY_EDITOR
            bannerViewID = bannerViewID_Test;
            interstitialAdID = interstitialID_Test;
        #else
            bannerViewID = isTesting ? bannerViewID_Test : bannerViewID_Live;
            interstitialAdID = isTesting ? interstitialID_Test : interstitialID_Live;
        #endif
    }

    public void InitializeAds()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Ads initialized");
        });
    }

    /// <summary>
    /// Creates and loads a banner ad at the bottom of the screen.
    /// </summary>
    public void LoadBannerAd()
    {
        if (bannerView != null)
        {
            DestroyBannerView();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerView = new BannerView(bannerViewID, adaptiveSize, AdPosition.Bottom);
        ListenToBannerAdEvents();

        AdRequest adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }

    private void ListenToBannerAdEvents()
    {
        if (bannerView != null)
        {
            bannerView.OnBannerAdLoaded += () => Debug.Log("Banner ad loaded.");
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) => Debug.LogError("Banner ad failed to load: " + error);
            bannerView.OnAdClicked += () => Debug.Log("Banner ad clicked.");
            bannerView.OnAdFullScreenContentClosed += () => Debug.Log("Banner full-screen content closed.");
        }
    }

    public void DestroyBannerView()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    /// <summary>
    /// Loads a Interstitial Video Ad.
    /// </summary>
    private void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading Interstitial Ad...");
        AdRequest adRequest = new AdRequest();

        InterstitialAd.Load(interstitialAdID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }

            interstitialAd = ad;
            Debug.Log("Interstitial ad loaded.");

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial Ad closed.");
                LoadInterstitialAd(); // Load new ad after closing
            };

            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"Interstitial Ad Paid: {adValue.Value} {adValue.CurrencyCode}");
            };
        });
    }

    /// <summary>
    /// Shows the Interstitial Ad if it's ready.
    /// </summary>
    private void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not available.");
        }
    }

    private void OnGameStart(object userData)
    {
        if (!Manager.Instance.tutorialEnabled)
        {
            LoadBannerAd();
            LoadInterstitialAd();
        }
    }

    private void OnEndGame(object userData)
    {
        if (!Manager.Instance.tutorialEnabled)
        {
            DestroyBannerView();
            ShowInterstitialAd();
        }
    }
}
