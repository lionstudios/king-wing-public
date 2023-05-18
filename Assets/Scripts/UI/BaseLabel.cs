using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BaseLabel : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateValue();
    }

    private void OnEnable()
    {
        UpdateValue();
    }

    private void Update()
    {
        UpdateValue();
    }

    public void UpdateValue()
    {
        text.text = GetTextValue();
    }

    protected abstract string GetTextValue();

}
