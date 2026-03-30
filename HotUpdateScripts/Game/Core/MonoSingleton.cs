using UnityEngine;

public class MonoSingleton<T> where T : MonoSingleton<T>, new()
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new T();
                m_instance.Initialize();
            }
            return m_instance;
        }
    }

    protected virtual void Initialize()
    {
    }
}
