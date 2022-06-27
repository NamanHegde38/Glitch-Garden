using System;
using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    [SerializeField] private GameObject winLabel, loseLabel;
    [SerializeField] private float waitToLoad;
    [SerializeField] private float waitToStart;

    [SerializeField] private AudioClip winSFX, loseSFX;
    [SerializeField] private MMFeedbacks winFeedback;
    [SerializeField] private MMFeedbacks loseFeedback;
    [SerializeField] private MMFeedbacks countdownFeedback;

    [SerializeField] private bool isBossLevel;
    [SerializeField] private bool isSurvivalLevel;
    [SerializeField] private int bossNumber;

    private int _numberOfAttackers;
    private bool _levelTimerFinished;
    private bool _hasLost;
    private bool _hasWon;
    private AttackerSpawner[] _spawnerArray;
    private AudioSource _audioSource;
    private LevelLoader _levelLoader;
    private MusicPlayer _musicPlayer;
    private float _masterVolume;

    public event EventHandler OnLevelStart;

    public void StartGame() {
        OnLevelStart?.Invoke(this, EventArgs.Empty);
    }

    public bool GetIsBossLevel() {
        return isBossLevel;
    }
    
    public bool GetIsSurvivalLevel() {
        return isSurvivalLevel;
    }
    
    private IEnumerator Start() {
        _spawnerArray = FindObjectsOfType<AttackerSpawner>();
        _musicPlayer = FindObjectOfType<MusicPlayer>();
        _audioSource = GetComponent<AudioSource>();
        _levelLoader = FindObjectOfType<LevelLoader>().GetComponent<LevelLoader>();
        _masterVolume = PlayerPrefsController.GetMasterVolume();

        if (winLabel) {
            winLabel.SetActive(false);
        }
        if (loseLabel) {
            loseLabel.SetActive(false);
        }

        yield return new WaitForSeconds(waitToStart);
        countdownFeedback.PlayFeedbacks();
    }

    public void AttackerSpawned() {
        _numberOfAttackers++;
    }

    public void AttackerKilled() {
        _numberOfAttackers--;

        if (_numberOfAttackers <= 0 && _levelTimerFinished) {
            StartCoroutine(HandleWinCondition());
        }
    }

    public void WinLevel() {
        Debug.Log("Level Won");
        StartCoroutine(HandleWinCondition());
    }

    private IEnumerator HandleWinCondition() {
        if (_hasLost) yield break;
        if (_hasWon) yield break;
        
        winLabel.SetActive(true);
        _hasWon = true;
        
        _musicPlayer.SetVolume(0f);
        _audioSource.volume = _masterVolume;
        _audioSource.clip = winSFX;
        _audioSource.Play();
        
        winFeedback.PlayFeedbacks();

        if (isBossLevel) {
            if (bossNumber > 0 && bossNumber <= 5) {
                switch (bossNumber) {
                    case 1:
                        PlayerPrefsController.FirstBossDefeated();
                        break;
                    case 2:
                        PlayerPrefsController.SecondBossDefeated();
                        break;
                    case 3:
                        PlayerPrefsController.ThirdBossDefeated();
                        break;
                    case 4:
                        PlayerPrefsController.FourthBossDefeated();
                        break;
                    case 5:
                        PlayerPrefsController.FifthBossDefeated();
                        break;
                }
            }
        }
        else {
            var currentLevel = SceneManager.GetActiveScene().buildIndex - 9;
            PlayerPrefsController.UnlockLevel(currentLevel + 1);
        }

        if (isSurvivalLevel) {
            var score = FindObjectOfType<ScoreDisplay>().GetScore();
            if (score > PlayerPrefsController.GetHighScore()) {}
            PlayerPrefsController.SetHighScore(score);
        }
        yield return new WaitForSeconds(waitToLoad);
        _levelLoader.LoadWinMenu();
    }

    public IEnumerator HandleLoseCondition() {
        if (_hasLost) yield break;
        if (_hasWon) yield break;
        
        loseLabel.SetActive(true);
        _hasLost = true;
        
        _musicPlayer.SetVolume(0f);
        _audioSource.volume = _masterVolume;
        _audioSource.clip = loseSFX;
        _audioSource.Play();
        
        loseFeedback.PlayFeedbacks();
        
        yield return new WaitForSeconds(waitToLoad);
        _levelLoader.LoadGameOver();
    }

    public void LevelTimerFinished() {
        _levelTimerFinished = true;
        StopSpawners();
    }

    private void StopSpawners() {
        foreach (var spawner in _spawnerArray) {
            spawner.StopSpawning();
        }
    }
}
