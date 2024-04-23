using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {
    
    [Tooltip("Time to finish the level in seconds")]
    private float _levelTime = 160f;

    private Slider _slider;
    private LevelController _levelController;
    private bool _triggeredLevelFinished;

    private int _difficulty = 1;

    private void Start() {
        _slider = GetComponent<Slider>();
        _levelController = FindObjectOfType<LevelController>();
        _difficulty = PlayerPrefsController.GetDifficulty();

        switch (_difficulty) {
            case 1:
                _levelTime *= 1f;
                break;
            case 2:
                _levelTime *= 1.5f;
                break;
            case 3:
                _levelTime *= 2f;
                break;
        }
    }

    private void Update() {
        if (_triggeredLevelFinished) return;
        _slider.value = Time.timeSinceLevelLoad / _levelTime;
        
        var timerFinished = Time.timeSinceLevelLoad >= _levelTime;
        
        if (!timerFinished) return;
        _levelController.LevelTimerFinished();
        _triggeredLevelFinished = true;
    }

    public float GetGameTime() {
        return Time.timeSinceLevelLoad / _levelTime;
    }

    public void SetGameTime(int startGameTime) {
        _levelTime = startGameTime;
    }
}
