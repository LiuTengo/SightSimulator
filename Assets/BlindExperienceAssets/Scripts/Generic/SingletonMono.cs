using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T _instance;

    public T instance => _instance;

    public virtual void Awake()
    {
        SetSingleton();
    }

    protected void SetSingleton()
    {
        if (instance != null)
        { 
            Destroy(instance.gameObject);
            _instance = null;
        }
        _instance = (T)this;
    }
}
