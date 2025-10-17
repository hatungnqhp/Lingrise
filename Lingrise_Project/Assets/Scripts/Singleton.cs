using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning($"");
            Destroy(gameObject);
            return;
        }
        instance = this as T;
        OnAwake();
    }

    protected virtual void OnAwake() { }
}