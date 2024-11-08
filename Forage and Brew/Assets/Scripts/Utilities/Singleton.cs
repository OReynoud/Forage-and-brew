using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T instance;

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
}
