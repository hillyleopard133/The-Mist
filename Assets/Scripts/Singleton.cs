using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <T> means it can be any type
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance {get; private set;}

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject); // Make it persist
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates 
        }
    }
}
