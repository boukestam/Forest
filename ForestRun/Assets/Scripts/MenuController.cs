using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    public float RunAnimationSpeed = 3f;

    private GameObject Dog;

    void Start() {
        Dog = GameObject.Find("Dog");
        Dog.GetComponent<Animation>()["Running"].speed = RunAnimationSpeed;
    }

    public void OnMainMenu() {
        LoadScene("Menu");
    }

    public void OnExitGame() {
        Application.Quit();
    }

    void LoadScene(string sceneName) {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene nextScene = SceneManager.GetSceneByName(sceneName);
        if (nextScene.IsValid()) {
            SceneManager.LoadScene(nextScene.buildIndex);
        }
    }
}
