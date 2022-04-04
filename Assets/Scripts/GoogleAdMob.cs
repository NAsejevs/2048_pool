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

            MobileAds.Initialize(initStatus => { });

            List<string> deviceIds = new List<string>();
            Debug.Log(SystemInfo.deviceUniqueIdentifier);
            deviceIds.Add(SystemInfo.deviceUniqueIdentifier);
            RequestConfiguration requestConfiguration = new RequestConfiguration
                .Builder()
                .SetTestDeviceIds(deviceIds)
                .build();

            MobileAds.SetRequestConfiguration(requestConfiguration);

            PlayGamesPlatform.Activate();
            PlayGamesPlatform.Instance.Authenticate((status) => { });
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
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-8598659276813255/7607042411";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-8598659276813255/1450143423";
        #else
            string adUnitId = "unexpected_platform";
        #endif

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
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-8598659276813255/7200752268";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-8598659276813255~6702470104";
        #else
            string adUnitId = "unexpected_platform";
        #endif

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
