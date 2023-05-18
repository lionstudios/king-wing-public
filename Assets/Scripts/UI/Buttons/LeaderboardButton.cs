using LionStudios.Suite.Analytics;
using UnityEngine;
using Utils;

public class LeaderboardButton : BaseButton
{


    protected override void OnClick()
    {
        UIManager.Instance.LeaderboardView.gameObject.SetActive(true);
        LionAnalytics.UiInteraction("Click", "LeaderboardButton", "CornerButtons_Bottom_Right", "Leaderboard");
    }
}