using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    [SerializeField] private float timeToWait = 3f;
    
    private int _currentSceneIndex;

    private void Start() {
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (_currentSceneIndex == 0) {
            StartCoroutine(WaitForTime());
        }
    }

    private IEnumerator WaitForTime() {
        yield return new WaitForSeconds(timeToWait);
        LoadNextScene();
    }

    public void RestartScene() {
        SceneManager.LoadScene(_currentSceneIndex);
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("Start Menu");
    }

    public void LoadLevelSelect() {
        SceneManager.LoadScene("Level Select Menu");
    }

    public void LoadLevel(int levelNumber) {
        SceneManager.LoadScene("Level " + levelNumber);
    }
    
    public void LoadOptions() {
        SceneManager.LoadScene("Options Menu");
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(_currentSceneIndex + 1);
    }

    public void LoadWinMenu() {
        SceneManager.LoadScene("Win Menu");
    }
    
    public void LoadGameOver() {
        SceneManager.LoadScene("Game Over");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
