using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono <T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static Object syncRootObject = new Object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                lock (syncRootObject)
                {
                    if (instance == null)
                    {
                        instance = new GameObject().AddComponent<T>();
                        instance.gameObject.name = instance.GetType().Name;
                    }
                }
            }
            return instance;
        }
    }

    public static bool IsEnable => instance != null;
}
