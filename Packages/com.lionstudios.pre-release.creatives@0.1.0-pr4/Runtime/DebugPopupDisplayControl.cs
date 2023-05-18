using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DebugPopupDisplayControl : MonoBehaviour
{
    private const float TIME_BETWEEN_TAPS = 1f;
    private const int TAPS_COUNT = 10;
    private const int TAP_FINGERS = 5;
    private int tapCounter = 0;
    private float lastTapTime;

    [RuntimeInitializeOnLoadMethod]
    public static void OnLoad()
    {
        Instantiate(Resources.Load("LionDebugConsole"));
    }
    
    private void Start()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        DebugLogManager.Instance.PopupEnabled = true;  
        DebugLogManager.Instance.HideLogWindow();
#else
        DebugLogManager.Instance.PopupEnabled = false;
#endif
        CheckEventSystems();
        SceneManager.sceneLoaded += (_, __) => CheckEventSystems();
    }

    void CheckEventSystems()
    {
        if (FindObjectsOfType<EventSystem>().Length > 1)
            GetComponentInChildren<EventSystem>().enabled = false;
    }

    private void Update()
    {
        
#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
        if (Input.touchCount == TAP_FINGERS && Input.touches.Any(t => t.phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.D))
        {
            if (Time.realtimeSinceStartup > lastTapTime + TIME_BETWEEN_TAPS)
                tapCounter = 0;
            tapCounter++;
            if (tapCounter == TAPS_COUNT)
            {
                DebugLogManager.Instance.PopupEnabled = !DebugLogManager.Instance.PopupEnabled;
                DebugLogManager.Instance.HideLogWindow();
                tapCounter = 0;
            }
            lastTapTime = Time.realtimeSinceStartup;
        }
#endif
    }
}
