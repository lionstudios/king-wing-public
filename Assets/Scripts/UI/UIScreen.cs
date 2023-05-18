using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour
{

    public void Show(bool enable)
    {
        if (enable)
            Show();
        else
            Hide();
    }

    public void Show()
    {
        Init();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Init() { }
    
}
