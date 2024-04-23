using System;
using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    [SerializeField] private float timeToWait = 3f;
    [SerializeField] private MMFeedbacks loadStartFeedback, loadEndFeedback;

    private Camera _fadeCamera;
    private int _currentSceneIndex;

    private void Start() {
        _fadeCamera = GameObject.FindWithTag("Fade Camera").GetComponent<Camera>();
        
        loadEndFeedback.Initialization();
        loadEndFeedback.PlayFeedbacks();

        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (_currentSceneIndex == 0) {
            StartCoroutine(LoadSceneIndex(_currentSceneIndex + 1, false));
        }
    }

    private IEnumerator LoadSceneString(string scene, bool hasTransition = true) {
        if (hasTransition && loadStartFeedback) {
            EnableFadeCamera();
            loadStartFeedback.PlayFeedbacks();
            yield return new WaitForSeconds(loadStartFeedback.TotalDuration);
            SceneManager.LoadScene(scene);
        }
        else {
            yield return new WaitForSeconds(timeToWait);
            SceneManager.LoadScene(scene);
        }
    }
    
    private IEnumerator LoadSceneIndex(int index, bool hasTransition = true) {
        if (hasTransition && loadStartFeedback) {
            EnableFadeCamera();
            loadStartFeedback.PlayFeedbacks();
            yield return new WaitForSeconds(loadStartFeedback.TotalDuration);
            SceneManager.LoadScene(index);
        }
        else {
            yield return new WaitForSeconds(timeToWait);
            EnableFadeCamera();
            loadStartFeedback.PlayFeedbacks();
            yield return new WaitForSeconds(loadStartFeedback.TotalDuration);
            SceneManager.LoadScene(index);
        }
    }
    
    public void RestartScene() {
        StartCoroutine(LoadSceneIndex(_currentSceneIndex));
    }

    public void LoadMainMenu() {
        StartCoroutine(LoadSceneString("Start Menu"));
    }

    public void LoadLevelSelect() {
        StartCoroutine(LoadSceneString("Level Select Menu"));
    }

    public void LoadLevel(int levelNumber) {
        var levelString = levelNumber.ToString();
        if (levelNumber < 10) {
            levelString = "0" + levelNumber;
        }
        StartCoroutine(LoadSceneString("Level " + levelString));
    }
    
    public void LoadBossLevel(int bossNumber) {
        var levelString = bossNumber.ToString();
        if (bossNumber < 10) {
            levelString = "0" + bossNumber;
        }
        StartCoroutine(LoadSceneString("Boss " + levelString));
    }
    
    public void LoadSurvivalLevel() {
        StartCoroutine(LoadSceneString("Survival"));
    }
    
    public void LoadOptions() {
        StartCoroutine(LoadSceneString("Options Menu"));
    }

    public void LoadNextScene() {
        StartCoroutine(LoadSceneIndex(_currentSceneIndex + 1));
    }

    public void LoadBossRushMenu() {
        StartCoroutine(LoadSceneString("Boss Rush Menu"));
    }
    
    public void LoadSurvivalMenu() {
        StartCoroutine(LoadSceneString("Survival Menu"));
    }
    
    public void LoadWinMenu() {
        StartCoroutine(LoadSceneString("Win Menu"));
    }
    
    public void LoadGameOver() {
        StartCoroutine(LoadSceneString("Game Over"));
    }

    public IEnumerator QuitGame() {
        EnableFadeCamera();
        loadStartFeedback.PlayFeedbacks();
        yield return new WaitForSeconds(loadStartFeedback.TotalDuration);
        Application.Quit();
    }

    private void EnableFadeCamera() {
        _fadeCamera.enabled = true;
    }
    
    private void DisableFadeCamera() {
        _fadeCamera.enabled = false;
    }
    
    private void OnEnable() {
        loadEndFeedback.Events.OnComplete.AddListener(DisableFadeCamera);
    }

    private void OnDisable() {
        loadEndFeedback.Events.OnComplete.RemoveListener(DisableFadeCamera);
    }
}
