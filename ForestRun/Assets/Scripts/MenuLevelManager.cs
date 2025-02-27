﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLevelManager : MonoBehaviour {
    public GameObject levelButtonTemplate;
    public Transform spacer;

    void Start() {
        FillList();
    }

    public void FillList() {
        LevelController.LoadLevels();
        foreach (var level in LevelManager.levels) {
            GameObject levelButton = Instantiate(levelButtonTemplate) as GameObject;
            LevelButton button = levelButton.GetComponent<LevelButton>();
            Button buttonBase = button.GetComponent<Button>();
            button.LevelText.text = level.levelNumber + "";

            if (PlayerPrefs.GetInt("Level" + level.levelNumber) == 1) {

                level.unlocked = true;
                level.isInteractable = true;
            }

            button.unlocked = level.unlocked;
            buttonBase.interactable = level.isInteractable;
            buttonBase.onClick.AddListener(() => LoadLevel(level.levelNumber));

            int score = PlayerPrefs.GetInt("Level" + level.levelNumber + "_score");
            int stars = Level.GetStars(score, (int)level.amountOfBones);
            if (stars == 1) {
                button.star1.SetActive(true);
            } else if (stars == 2) {
                button.star1.SetActive(true);
                button.star2.SetActive(true);
            } else if (stars == 3) {
                button.star1.SetActive(true);
                button.star2.SetActive(true);
                button.star3.SetActive(true);
            }
            levelButton.transform.SetParent(spacer.transform, false);
        }
        SaveAll();
    }

    private void Update() {
        GameObject[] allLevelButtons = GameObject.FindGameObjectsWithTag("LevelButton");
        foreach (var levelButton in allLevelButtons) {
            LevelButton button = levelButton.GetComponent<LevelButton>();
            Button buttonBase = button.GetComponent<Button>();
            int levelnumber = int.Parse(button.LevelText.text);

            if (PlayerPrefs.GetInt("Level" + levelnumber) == 1) {
                
                button.unlocked = true;
                buttonBase.interactable = true;
            } else {

                button.unlocked = false;
                buttonBase.interactable = false;
            }            
        }
    }

    void SaveAll() {
        GameObject[] allLevelButtons = GameObject.FindGameObjectsWithTag("LevelButton");
        foreach (var levelButton in allLevelButtons) {
            LevelButton button = levelButton.GetComponent<LevelButton>();
            if (LevelManager.levels[int.Parse(button.LevelText.text) - 1].unlocked) {
                PlayerPrefs.SetInt("Level" + button.LevelText.text, 1);
            }
        }
        PlayerPrefs.Save();
    }

    public void DeleteAll() {
        PlayerPrefs.DeleteAll();
        DeleteButtons();
        FillList();
    }

    void DeleteButtons() {
        foreach (Transform child in spacer) {
            Destroy(child.gameObject);
        }
    }

    void LoadLevel(int levelNumber) {
        PlayerPrefs.SetInt("lastPlayedLevel", levelNumber);
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        Scene nextScene = SceneManager.GetSceneByName("Main");
        if (nextScene.IsValid()) {
            SceneManager.LoadScene(nextScene.buildIndex);
        }
    }
}
