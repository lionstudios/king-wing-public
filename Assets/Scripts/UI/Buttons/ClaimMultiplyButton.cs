using System;
using Events.Level.EventArgs;
using UnityEngine;
using Utils;
using EventType = LionStudios.Suite.Analytics.EventType;

public class ClaimMultiplyButton : BaseButton
{
    [SerializeField] private int rewardMultiplier;

    protected override void OnClick()
    {
        var rewardArgs = new RewardArgs(RewardType.MultiplierReward, rewardMultiplier);
        _dispatcher.Send(EventId.SendRewardData, rewardArgs);
        AdsManager.HideCrossPromo();

        LevelEventArgs levelEventArgs = new LevelEventArgs
        {
            LevelNum = LevelManager.Instance.CurrentLevel,
            Score = GameManager.Instance.LevelMoney,
            AttemptNum = GameManager.Instance.Attempts,
            EventType = EventType.Ad
        };

        AdsManager.ShowRewardedAd(levelEventArgs);
        
    }
}