using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance {get => instance;
        private set { }
    }
    private static SceneLoader instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        instance = this;
    }

    public AsyncOperation LoadGameScene()
    {
        return SceneManager.LoadSceneAsync("GameScene");
    }
}
