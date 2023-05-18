using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;


public class EditorMenu : EditorWindow
{
    private static string LocationForSS
    {
        get => PlayerPrefs.GetString("Path", Application.dataPath);
        set => PlayerPrefs.SetString("Path", value);
    }
    
    private static string SaveFolder => LocationForSS + "/ScreenShots";

    private static void SaveStuff()
    {
        if (!Directory.Exists(SaveFolder))
        {
            Directory.CreateDirectory(SaveFolder);
        }
    }


    [MenuItem("ScreenShot Taker/ScreenShots")]
    private static void ShowWindow()
    {
        var window = GetWindow<EditorMenu>("Editor Menu");
        window.Show();
    }


    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Persistent Path"))
        {
            Application.OpenURL(Application.persistentDataPath);
        }

        if (GUILayout.Button("Delete Persistent Files"))
        {
            Directory.Delete(Application.persistentDataPath, true);
        }

        GUILayout.EndHorizontal();

        LocationForSS =
            EditorGUILayout.TextField("Save Location:", LocationForSS);
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.Space(20);

        if (GUILayout.Button("Save SS Regular"))
        {
            SaveStuff();
            CaptureScreenshot();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Save SS 2X"))
        {
            SaveStuff();
            CaptureScreenshot(2);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Save SS 3X"))
        {
            SaveStuff();
            CaptureScreenshot(3);
        }

        GUILayout.EndVertical();
    }

    [MenuItem("ScreenShot Taker/NormalSS Cmd+Shift+E  %#e")]
    public static void TakeNormalSS()
    {
        SaveStuff();
        CaptureScreenshot();
    }
    private static void CaptureScreenshot(int superSize = 1)
    {
        ScreenCapture.CaptureScreenshot(
            SaveFolder + "/IMG-" + ScreenResolution() + "-" + GetCurrentTime() + ".png",
            superSize);
        AssetDatabase.Refresh();
        Application.OpenURL(SaveFolder);
    }

    private static string ScreenResolution()
    {
        return UnityEngine.Screen.width + "_" + UnityEngine.Screen.height;
    }

    private static string GetCurrentTime()
    {
        var currentTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        currentTime = currentTime.Replace("/", "-");
        currentTime = currentTime.Replace(" ", "-");
        currentTime = currentTime.Replace(":", "-");
        return currentTime;
    }
}