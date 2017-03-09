using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MenuLevelManager : MonoBehaviour
{

    [System.Serializable]
    public class Level
    {
        public int levelNumber;
        public int unlocked;
        public bool isInteractable;
    }

    public GameObject levelButtonTemplate;
    public Transform spacer;
    public List<Level> levelList;

    // Use this for initialization
    void Start()
    {
        FillList();
    }

    void FillList()
    {
        foreach (var level in levelList)
        {
            GameObject levelButton = Instantiate(levelButtonTemplate) as GameObject;
            LevelButton button = levelButton.GetComponent<LevelButton>();
            Button buttonBase = button.GetComponent<Button>();
            button.LevelText.text = level.levelNumber + "";

            if (PlayerPrefs.GetInt("Level" + level.levelNumber) == 1)
            {
                level.unlocked = 1;
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
                button.star2.SetActive(true);
            }

            levelButton.transform.SetParent(spacer, false);
        }

        SaveAll();
    }

    void SaveAll()
    {
        //if (!PlayerPrefs.HasKey("Level1"))
        GameObject[] allLevelButtons = GameObject.FindGameObjectsWithTag("LevelButton");
        foreach (var levelButton in allLevelButtons)
        {
            LevelButton button = levelButton.GetComponent<LevelButton>();
            PlayerPrefs.SetInt("Level" + button.LevelText.text, button.unlocked);
        }
    }

    void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    void LoadLevel(int levelNumber)
    {
        //TODO LOAD LEVEL CORESPONDING TO levelNumber
    }
}
