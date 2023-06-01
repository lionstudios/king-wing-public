using System;
using System.Collections.Generic;
using Events.Level.EventArgs;
using IngameDebugConsole;
using LionStudios.OfflineModule;
using UnityEngine;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Leaderboards;
using Utils;
using Debug = UnityEngine.Debug;
using EventType = LionStudios.Suite.Analytics.EventType;
using Product = LionStudios.Suite.Analytics.Product;
using Task = System.Threading.Tasks.Task;

public class GameManager : MonoSingleton<GameManager>
{
    private const string ATTEMPTS_PREFS_KEY = "Attempts";

    private const string NO_ADS_PURCHASED_PREFS_KEY = "NoAds";

    private IDisposable _rewardManager;

    public static Dispatcher Dispatcher { get; } = new Dispatcher();

    public int LevelMoney { get; private set; }

    public bool IsStarted
    {
        get => _isStartedGame;
        set
        {
            _isStartedGame = value;
            GamePlayStatus?.Invoke(_isStartedGame);
        }
    }

    private bool _isStartedGame;

    public static Action<bool> GamePlayStatus;

    public int Attempts
    {
        get => PlayerPrefs.GetInt(ATTEMPTS_PREFS_KEY, 0);
        private set => PlayerPrefs.SetInt(ATTEMPTS_PREFS_KEY, value);
    }

    private int AdsPurchasedCheck
    {
        get => PlayerPrefs.GetInt(NO_ADS_PURCHASED_PREFS_KEY, 0);
        set => PlayerPrefs.SetInt(NO_ADS_PURCHASED_PREFS_KEY, value);
    }

    public Character lionCharacter;
    public CameraController cameraController;
    public InfiniteManager infiniteManager;
    public GameObject skipToEndBtn;

    private void Awake()
    {
#if !UNITY_EDITOR
                Application.targetFrameRate = 60;
#endif
        _rewardManager = new RewardManager(Dispatcher);
        Dispatcher.Subscribe(EventId.ResetLevel, ResetLevelScore);

        //schedule test notification
        NotificationManager.Instance.Schedule(new TimeSpan(0, 24, 0, 00), "kingwing_notification",
            "KingWing Notification",
            "Flap your wings to victory!", true);
        GameManager.Instance.PauseGamePlayElements();

        DebugLogConsole.AddCommand("skipbuttonON", "Turn On Skip Button", SkipBtnTurnOn);
        DebugLogConsole.AddCommand("skipbuttonOFF", "Turn Off Skip Button", SkipBtnTurnOff);
    }

    private async void Start()
    {
        await Task.Delay(2000);
        AdsManager.ShowBannerAd();
        AdsManager.ShowCrossPromo();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _rewardManager.Dispose();
        Dispatcher.Unsubscribe(EventId.ResetLevel, ResetLevelScore);
    }

    public void StartGame()
    {
        if (LevelManager.Instance.CurrentLevel == 1)
        {
            LionAnalytics.SetTutorial(true, "Level1");
            LionAnalytics.AbCohort("TutorialLevel", "TestCohort");
            LionAnalytics.GameStart();
            LionAnalytics.LevelStep(LevelManager.Instance.CurrentLevel, LevelMoney, "Level", "Level", "Normal", "Level",
                Attempts);
            LionAnalytics.MissionStarted(true, "Normal", "Level", "LevelID", Attempts);
            LionAnalytics.AbCohort("TutorialLevel", "TestCohort");
            LionAnalytics.ClearAbCohort("TutorialLevel");
            LionAnalytics.NewPlayer();
            LionAnalytics.SetTutorial(false, "Level1");
        }
        else
        {
            LionAnalytics.SetHardCurrency(CurrencyManager.Instance.TotalMoney);
            LionAnalytics.SetSoftCurrency(CurrencyManager.Instance.TotalMoney);
            LionAnalytics.SetPlayerLevel(LevelManager.Instance.CurrentLevel);
            LionAnalytics.SetPlayerXP(CurrencyManager.Instance.TotalMoney);
            LionAnalytics.SetPlayerScore(CurrencyManager.Instance.TotalMoney);
            LionAnalytics.LevelStart(LevelManager.Instance.CurrentLevel, Attempts, LevelMoney, "Level", "Level",
                "Normal",
                "Level");
            LionAnalytics.MissionStarted(false, "Normal", "Level", "LevelID", Attempts);
            LionAnalytics.PredictionResult("modelName", "modelVersion", "input", "output");

            LionAnalytics.DebugEvent("Level Started!");
        }


        SatoriSDK.SatoriLevelStartEvent(LevelManager._instance.CurrentLevel, null);

        //Play the animation on the lion running on the platform
        // Character.Instance._animator.SetTrigger(Character.Run);

        AdsManager.HideCrossPromo();


        // LION OFFLINE MODULE LOGIC
        // Turn back on offline monitoring
        OfflineDetection.SetInterruptable(true);
        Debug.Log("TURNING OFFLINE MONITORING OFF. LEVEL STARTED");

        IsStarted = true;
    }

    public void PauseGamePlayElements()
    {
        IsStarted = false;
    }

    public void IncrementScore()
    {
        LevelMoney++;
    }

    public void Succeed()
    {
        GameManager.Instance.PauseGamePlayElements();
        LionAnalytics.LevelUp(LevelManager.Instance.CurrentLevel.ToString(), null);
        var product = new Product();
        product.AddItem("Money", "Reward", LevelMoney);
        product.virtualCurrencies.Add(new VirtualCurrency("Coins", "reward", LevelMoney));
        product.realCurrency = new RealCurrency("none", 0);

        var reward = new Reward(product);

        var levelArgs = new LevelArgs(reward, LevelManager.Instance.CurrentLevel, Attempts, LevelMoney, "",
            "",
            "Normal", "Level", "LevelID", "LevelSuccess", "CompleteLevel");

        if (LevelManager.Instance.CurrentLevel == 3 || LevelManager.Instance.CurrentLevel == 7)
        {
            Dispatcher.Send(EventId.InAppReviewRequest, null);
        }

        LevelEventArgs levelEventArgs = new LevelEventArgs
        {
            LevelNum = LevelManager.Instance.CurrentLevel,
            Score = LevelMoney,
            AttemptNum = Attempts,
            EventType = EventType.Ad
        };
        SatoriSDK.SatoriLevelCompleteEvent(LevelManager._instance.CurrentLevel, null);
        AnalyticsManager.OnLevelCompleted(levelArgs);
    
        //Dispatcher.Send(EventId.LaunchInterstitialVideo, levelEventArgs);
        AdsManager.ShowInterstitialAd(levelArgs);

        Dispatcher.Send(EventId.IncrementLevel, EventArgs.Empty);
        AdsManager.ShowCrossPromo();
        Attempts = 0;
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Succeed));

    }

    public async void Fail()
    {
        LevelEventArgs levelEventArgs = new LevelEventArgs
        {
            LevelNum = LevelManager.Instance.CurrentLevel,
            Score = LevelMoney,
            AttemptNum = Attempts,
            EventType = EventType.Ad
        };


        GameManager.Instance.PauseGamePlayElements();
        LionAnalytics.LevelFail(LevelManager.Instance.CurrentLevel, Attempts, LevelMoney, "Level", "Level", "Normal",
            "Level");
        LionAnalytics.MissionFailed(false, "Normal", "Level", "LevelID", LevelMoney, Attempts);

        SatoriSDK.SatoriLevelFailEvent(LevelManager._instance.CurrentLevel, null);
        Attempts++;
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Fail));


        //Dispatcher.Send(EventId.LaunchInterstitialVideo, levelEventArgs);
        AdsManager.ShowInterstitialAd(levelEventArgs);
        AdsManager.ShowCrossPromo();





        // LION OFFLINE MODULE LOGIC
        // Turn back on offline monitoring
        OfflineDetection.SetInterruptable(false);
        Debug.Log("TURNING OFFLINE MONITORING BACK ON");




        UIManager.Instance.LeaderboardView.gameObject.SetActive(true);
        List<LeaderboardGroup> leaderboards = await LeaderboardsManager.ListLeaderboards();
        LeaderboardGroup leaderboardGroup = leaderboards[0];
        await LeaderboardsManager.RecordScore(leaderboardGroup.subLeaderboards[0].id, LevelMoney);
    }


    private void ResetLevelScore(EventArgs args)
    {
        LevelMoney = 0;
    }

    public void NoAdsPurchased()
    {
        AdsPurchasedCheck = 1;
        OfflineDetection.SetNoAds(true);
        Dispatcher.Send(EventId.RemoveNoAdsButtonClicked, EventArgs.Empty);
        AdsManager.HideBannerAd();
        AdsManager.HideCrossPromo();

    }


    public bool CheckNoAdPurchase()
    {
        return AdsPurchasedCheck == 1;
    }

    private void SkipBtnTurnOn()
    {
        skipToEndBtn.SetActive(true);
    }

    private void SkipBtnTurnOff()
    {
        skipToEndBtn.SetActive(false);
    }
    
}