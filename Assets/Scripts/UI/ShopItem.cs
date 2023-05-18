using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LionStudios.Suite.Analytics;

[CreateAssetMenu]
public class ShopItem : ScriptableObject
{
    private const string UNLOCKED_KEY_PREFIX = "ShopItemUnlocked";
    
    [SerializeField] public Sprite icon;
    [SerializeField] public string id;
    [SerializeField] public string itemName;
    [SerializeField] public int price;
    [SerializeField] public bool unlockedOnStart;
    
    public Action OnUnlocked;
    public Action OnReset;
    
    private string UnlockedKey => $"{UNLOCKED_KEY_PREFIX}_{id}";

    public bool IsUnlocked
    {
        get
        {
            if (unlockedOnStart)
                return true;
            return PlayerPrefs.GetInt(UnlockedKey, 0) > 0;
        }
    }

    public void Unlock()
    {
        Reward reward = new Reward("Skin", "Unlockable", 1);
        LionAnalytics.LevelUp("SkinUnlocked", reward);
        PlayerPrefs.SetInt(UnlockedKey, 1);
        OnUnlocked?.Invoke();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(UnlockedKey);
        OnReset?.Invoke();
    }

    [ContextMenu("SetupIdFromObjectName")]
    public void SetupIdFromName()
    {
        id = name;
    }

    [ContextMenu("SetupNameFromObjectName")]
    public void SetupNameFromName()
    {
        itemName = name;
    }
    
}
