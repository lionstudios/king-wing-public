using UnityEngine;
using UnityEngine.UI;
using Utils;

public abstract class BaseButton : MonoBehaviour
{
    protected Dispatcher _dispatcher;
    
    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        _dispatcher = GameManager.Dispatcher;
    }

    protected abstract void OnClick();

}
