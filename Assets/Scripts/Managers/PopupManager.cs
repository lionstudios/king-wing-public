using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoSingleton<PopupManager>
{
    
    [SerializeField] private GameObject _popupContainer;

    [SerializeField] private TextMeshProUGUI _text;

    public bool isShowing { get; private set; }
    
    private float _timeToClose = float.MaxValue;

    public void ShowPopup(string text)
    {
        _popupContainer.SetActive(true);
        _text.text = text;
        isShowing = true;
    }
    public void HidePopup()
    {
        _popupContainer.SetActive(false);
        isShowing = false;
    }
    
    public void ShowPopupTemporarily(string text, float time)
    {
        _popupContainer.SetActive(true);
        _text.text = text;
        _timeToClose = Time.realtimeSinceStartup + time;
        isShowing = true;
    }

    private void Update()
    {
        if (isShowing && Time.realtimeSinceStartup >= _timeToClose)
        {
            HidePopup();
        }
    }
}
