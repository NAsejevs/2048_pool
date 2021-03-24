using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

public class GoogleAdMob : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitial;

    public static GoogleAdMob googleAdMob;

    void Awake()
    {
        if (googleAdMob == null)
        {
            googleAdMob = this;
            DontDestroyOnLoad(this);

            string appId = "ca-app-pub-8598659276813255~4720368080";

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(appId);

            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

            PlayGamesPlatform.InitializeInstance(config);
            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = true;
            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();

            Social.localUser.Authenticate((bool success) => {
                // handle success or failure
            });
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.RequestBanner();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestBanner()
    {
        // string adUnitId = "ca-app-pub-3940256099942544/6300978111"; // test ad
        string adUnitId = "ca-app-pub-8598659276813255/7607042411";

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
        bannerView.Show();
    }

    public void RequestInterstitial()
    {
        //string adUnitId = "ca-app-pub-3940256099942544/1033173712"; // test ad
        string adUnitId = "ca-app-pub-8598659276813255/7200752268";

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
        interstitial.OnAdLoaded += HandleOnInterstitialLoaded;
    }

    public void HandleOnInterstitialLoaded(object sender, EventArgs args)
    {
        interstitial.Show();
    }
}
