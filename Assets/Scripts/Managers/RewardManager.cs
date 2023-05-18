using System;
using Utils;

public class RewardArgs : EventArgs
{
    public RewardType Reward { get; }
    public int RewardMultiplier { get; }

    public RewardArgs(RewardType rewardType, int multiplier = 0)
    {
        Reward = rewardType;
        RewardMultiplier = multiplier;
    }
}

public enum RewardType
{
    SkipReward,
    MultiplierReward
}

public class RewardManager : IDisposable
{
    private readonly Dispatcher _dispatcher;

    private RewardArgs currentRewardArgs;

    public RewardManager(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _dispatcher.Subscribe(EventId.SendRewardData, RewardData);
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedReward;
    }

    void IDisposable.Dispose()
    {
        _dispatcher.Unsubscribe(EventId.SendRewardData, RewardData);
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedReward;
    }


    private void OnRewardedAdReceivedReward(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        ClaimReward(currentRewardArgs);
    }

    private void RewardData(EventArgs args)
    {
        currentRewardArgs = (RewardArgs)args;
    }

    private void ClaimReward(RewardArgs args)
    {
        switch (args.Reward)
        {
            case RewardType.SkipReward:
                SkipReward();
                break;
            case RewardType.MultiplierReward:
                MultipliedReward(args.RewardMultiplier);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SkipReward()
    {
        _dispatcher.Send(EventId.AddMoney, new MoneyArgs(GameManager.Instance.LevelMoney));
        _dispatcher.Send(EventId.IncrementLevel, EventArgs.Empty);
        ResetLevelStuff();
    }

    private void MultipliedReward(int multiplier)
    {
        _dispatcher.Send(EventId.AddMoney, new MoneyArgs(GameManager.Instance.LevelMoney * multiplier));
        ResetLevelStuff();
    }


    private void ResetLevelStuff()
    {
        _dispatcher.Send(EventId.ResetLevel, EventArgs.Empty);
        UIManager.Instance.SetScreen(new UiSwitchArgs(Screen.Continue));
    }
}