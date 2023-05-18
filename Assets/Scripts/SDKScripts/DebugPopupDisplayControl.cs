using System.Linq;
using IngameDebugConsole;
using UnityEngine;

public class DebugPopupDisplayControl : MonoBehaviour
{
    private const float TIME_BETWEEN_TAPS = 1f;
    private const int TAPS_COUNT = 10;
    private const int TAP_FINGERS = 3;
    private int tapCounter = 0;
    private float lastTapTime;

    [RuntimeInitializeOnLoadMethod]
    public static void OnLoad()
    {
        Instantiate(Resources.Load("LionDebugConsole"));
    }

    private void Start()
    {
        DebugLogManager.Instance.PopupEnabled = true;
        DebugLogManager.Instance.HideLogWindow();
        lastTapTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if ((Input.touchCount < TAP_FINGERS || Input.touches.All(t => t.phase != TouchPhase.Began)) &&
            !Input.GetKeyDown(KeyCode.D)) return;

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
}