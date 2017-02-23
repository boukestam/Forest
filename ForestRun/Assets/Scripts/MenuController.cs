using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    private bool loadLock = false;
    string nextSceneName;
    AsyncOperation async;

    private GameObject Player;
    

    public void StartGame(string sceneName)
    {
        if (!loadLock)
        {
            nextSceneName = sceneName;
            async = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
            if (nextScene.IsValid())
            {
                SceneManager.LoadScene(nextScene.buildIndex);
            }
            loadLock = true;
        }
    }
    

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }
    
    void FixedUpdate()
    {
        Player.transform.position = new Vector3(
            Player.transform.position.x,
            Player.transform.position.y,
            Player.transform.position.z + (10.0f * Time.deltaTime)
        );
    }
}
