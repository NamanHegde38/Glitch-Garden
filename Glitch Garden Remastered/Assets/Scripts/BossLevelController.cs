using System;
using System.Collections;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLevelController : MonoBehaviour {

    [SerializeField] private GameObject winLabel, loseLabel;
    [SerializeField] private float waitToLoad;

    [SerializeField] private AudioClip winSFX, loseSFX;
    [SerializeField] private MMFeedbacks winFeedback;
    [SerializeField] private MMFeedbacks loseFeedback;
    [SerializeField] private MMFeedbacks countdownFeedback;

    [SerializeField] private int bossNumber = 1;

    private int _numberOfAttackers;
    private bool _levelTimerFinished;
    private bool _hasLost;
    private bool _hasWon;
    private AttackerSpawner[] _spawnerArray;
    private AudioSource _audioSource;
    private LevelLoader _levelLoader;
    private float _masterVolume;

    public event EventHandler OnLevelStart;
    
    public void StartGame() {
        OnLevelStart?.Invoke(this, EventArgs.Empty);
    }
    
    private void Start() {
        countdownFeedback.Initialization();
        countdownFeedback.PlayFeedbacks();
        _spawnerArray = FindObjectsOfType<AttackerSpawner>();
        _audioSource = GetComponent<AudioSource>();
        _levelLoader = FindObjectOfType<LevelLoader>().GetComponent<LevelLoader>();
        _masterVolume = PlayerPrefsController.GetMasterVolume();

        if (winLabel) {
            winLabel.SetActive(false);
        }
        if (loseLabel) {
            loseLabel.SetActive(false);
        }
    }

    public void AttackerSpawned() {
        _numberOfAttackers++;
    }

    public void AttackerKilled() {
        _numberOfAttackers--;

        if (_numberOfAttackers <= 0 && _levelTimerFinished) {
            //StartCoroutine(HandleWinCondition());
        }
    }

    public IEnumerator HandleWinCondition() {
        if (_hasLost) yield break;
        if (_hasWon) yield break;
        
        GameObject.FindWithTag("Music Player").GetComponent<MMFeedbacks>().StopFeedbacks();
        
        winLabel.SetActive(true);
        _hasWon = true;

        winFeedback.PlayFeedbacks();

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

        yield return new WaitForSeconds(waitToLoad);
        _levelLoader.LoadWinMenu();
    }

    public IEnumerator HandleLoseCondition() {
        if (_hasLost) yield break;
        if (_hasWon) yield break;
        
        GameObject.FindWithTag("Music Player").GetComponent<MMFeedbacks>().StopFeedbacks();
        
        loseLabel.SetActive(true);
        _hasLost = true;

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
