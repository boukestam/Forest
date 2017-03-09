using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLevelManager : MonoBehaviour
{

    /*[System.Serializable]
    public class Level
    {
        public int levelNumber;
        public int unlocked;
        public bool isInteractable;
    }*/

    public GameObject levelButtonTemplate;
    public Transform spacer;
    //public List<Level> levelList;

    // Use this for initialization
    void Start()
    {        
        FillList();
    }

    void FillList()
    {
        LevelController.LoadLevels();
        foreach (Button child in spacer)
        {
            Destroy(child);
        }

        foreach (var level in LevelManager.levels)
        {
            GameObject levelButton = Instantiate(levelButtonTemplate) as GameObject;
            LevelButton button = levelButton.GetComponent<LevelButton>();
            Button buttonBase = button.GetComponent<Button>();
            button.LevelText.text = level.levelNumber + "";

            if (PlayerPrefs.GetInt("Level" + level.levelNumber) == 1)
            {
                level.unlocked = true;
                level.isInteractable = true;
            }

            button.unlocked = level.unlocked;
            buttonBase.interactable = level.isInteractable;
            buttonBase.onClick.AddListener(() => LoadLevel(level.levelNumber));

            int score = PlayerPrefs.GetInt("Level" + level.levelNumber + "_score");

            if (score > 0)
            {
                button.star1.SetActive(true);
            }
            else if (score > 10)
            {
                button.star2.SetActive(true);
            }
            else if (score > 100)
            {
                button.star3.SetActive(true);
            }

            levelButton.transform.SetParent(spacer, false);
        }

        SaveAll();
    }

    void SaveAll()
    {
        GameObject[] allLevelButtons = GameObject.FindGameObjectsWithTag("LevelButton");
        foreach (var levelButton in allLevelButtons)
        {
            LevelButton button = levelButton.GetComponent<LevelButton>();
            if (button.unlocked)
            {
                PlayerPrefs.SetInt("Level" + button.LevelText.text, 1);
            }
        }
    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        FillList();
    }

    void LoadLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("lastPlayedLevel", levelNumber);
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        Scene nextScene = SceneManager.GetSceneByName("Main");
        if (nextScene.IsValid())
        {
            SceneManager.LoadScene(nextScene.buildIndex);
        }
    }
}
