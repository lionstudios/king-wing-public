using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{

	protected static T _instance = null;
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(T)) as T;
			}
			return _instance;
		}
	}

	protected virtual void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	void Awake()
	{
		_instance = (T)this;
		OnAwake();
	}

	protected virtual void OnAwake() { }
}