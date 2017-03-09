using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    
    public enum MenuStates { Main, Levels };
    public MenuStates currentState;


    public float ForwardSpeed = 1;
    public float JumpForce = 1;
    public float RunAnimationSpeed = 3f;

    private bool loadLock = false;
    string nextSceneName;
    AsyncOperation async;

    private GameObject Player;
    private GameObject Dog;

    private GameObject mainMenu;
    private GameObject levelMenu;

    void Awake()
    {
        currentState = MenuStates.Main;
        
    }

    public void OnStartGame(string sceneName)
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

    public void OnSelectLevel()
    {
        currentState = MenuStates.Levels;
    }

    public void OnMainMenu()
    {
        currentState = MenuStates.Main;
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

    void Start()
    {
        Player = GameObject.FindWithTag("Player");

        Dog = GameObject.Find("Dog");
        Dog.GetComponent<Animation>()["Running"].speed = RunAnimationSpeed;

        mainMenu = GameObject.FindWithTag("MainMenu");
        levelMenu = GameObject.FindWithTag("LevelMenu");
    }

    void Update()
    {
        switch (currentState)
        {
            case MenuStates.Main:
                mainMenu.SetActive(true);
                levelMenu.SetActive(false);
                break;
            case MenuStates.Levels:
                mainMenu.SetActive(false);
                levelMenu.SetActive(true);
                break;
        }        
    }

    void FixedUpdate()
    {
        Player.transform.position = new Vector3(
            Player.transform.position.x,
            Player.transform.position.y,
            Player.transform.position.z + (10.0f * Time.fixedDeltaTime)
        );
    }
}
