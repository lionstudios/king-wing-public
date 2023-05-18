using System;
using UnityEngine;
using Utils;

public class LevelManager : MonoSingleton<LevelManager>
{
    private const string LEVEL_PREFS_KEY = "Level";

    private int _currentLevelCached = -1;
    private Dispatcher _dispatcher;

    public int CurrentLevel
    {
        get
        {
            if (_currentLevelCached < 0)
                _currentLevelCached = PlayerPrefs.GetInt(LEVEL_PREFS_KEY, 1);
            return _currentLevelCached;
        }
        private set
        {
            PlayerPrefs.SetInt(LEVEL_PREFS_KEY, value);
            _currentLevelCached = value;
        }
    }
     
    private void Start()
    {
        _dispatcher = GameManager.Dispatcher;
        _dispatcher.Subscribe(EventId.IncrementLevel, NextLevel);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _dispatcher.Unsubscribe(EventId.IncrementLevel, NextLevel);
    }

    private void NextLevel(EventArgs args)
    {
        CurrentLevel++;
    }
}