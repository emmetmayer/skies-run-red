using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance;

    [SerializeField] private GameObject CTFManagerObject;
    [SerializeField] private GameObject NetManagerObject;
    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
        
        DontDestroyOnLoad(gameObject);
        
        Instantiate(CTFManagerObject).transform.SetParent(transform);
        Instantiate(NetManagerObject).transform.SetParent(transform);
    }

    [Range(-4, 2)] private float offset;
}
