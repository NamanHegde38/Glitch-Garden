using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {
    
    [Tooltip("Time to finish the level in seconds")]
    [SerializeField] private float levelTime = 10f;

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
                levelTime *= 1f;
                break;
            case 2:
                levelTime *= 1.5f;
                break;
            case 3:
                levelTime *= 2f;
                break;
        }
    }

    private void Update() {
        if (_triggeredLevelFinished) return;
        _slider.value = Time.timeSinceLevelLoad / levelTime;
        
        var timerFinished = Time.timeSinceLevelLoad >= levelTime;
        
        if (!timerFinished) return;
        _levelController.LevelTimerFinished();
        _triggeredLevelFinished = true;
    }

    public float GetGameTime() {
        return Time.timeSinceLevelLoad / levelTime;
    }
}
