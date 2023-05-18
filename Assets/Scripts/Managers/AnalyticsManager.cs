using System;
using LionStudios.OfflineModule;
using LionStudios.Suite.Analytics;
using Utils;

public class LevelArgs : EventArgs
{
    public Reward Reward { get; }
    public int LevelNum { get; }
    public int AttemptNum { get; }
    public int Score { get; }
    public string LevelCollection1 { get; }
    public string LevelCollection2 { get; }
    public string MissionType { get; }
    public string MissionName { get; }
    public string MissionID { get; }
    public string AchievementID { get; }
    public string AchievementName { get; }

    public LevelArgs(Reward reward, int levelNum, int attemptNum, int score, string levelCollection1,
        string levelCollection2, string missionType, string missionName, string missionID, string achievementID,
        string achievementName)
    {
        Reward = reward;
        LevelNum = levelNum;
        AttemptNum = attemptNum;
        Score = score;
        LevelCollection1 = levelCollection1;
        LevelCollection2 = levelCollection2;
        MissionType = missionType;
        MissionName = missionName;
        MissionID = missionID;
        AchievementID = achievementID;
        AchievementName = achievementName;
    }
}

public class AnalyticsManager
{ 
    public static void OnLevelCompleted(EventArgs a)
    {
        var args = (LevelArgs) a;

        LionAnalytics.LevelComplete(LevelManager.Instance.CurrentLevel, args.AttemptNum, args.Score, args.Reward,
            args.LevelCollection1, args.LevelCollection2, args.MissionType, args.MissionName);
        LionAnalytics.MissionCompleted(false, args.MissionType, args.MissionName, args.MissionID, args.AttemptNum,
            args.Score);
        LionAnalytics.Achievement(args.Reward, args.AchievementID, args.AchievementName);
    }
}