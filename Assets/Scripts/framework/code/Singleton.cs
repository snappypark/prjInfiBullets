using UnityEngine;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
    protected static T _inst;
    public static T Inst
    {
        get
        {
            if (_inst == null)
                _inst = new T();
            return _inst;
        }
    }
    public static bool Exists
    {
        get
        {
            return _inst != null;
        }
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _inst;

    private static object _sLock = new object();

    private static bool m_sAppIsQuitting = false;

    public static T InstWithNoCreate
    {
        get
        {
            return _inst;
        }
    }

    public static T Inst
    {
        get
        {
            if (m_sAppIsQuitting)
                return null;

            lock (_sLock)
            {
                if (_inst == null)
                {
                    _inst = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _inst;
                    }

                    if (_inst == null)
                    {
                        GameObject singleton = new GameObject();
                        _inst = singleton.AddComponent<T>();
                        singleton.name = "(Singleton)" + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {
                    }
                }

                return _inst;
            }
        }
    }

    public void OnDestroy()
    {
        m_sAppIsQuitting = true;
    }
}
