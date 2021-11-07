using UnityEngine;
using TMPro;

public class LivesDisplay : MonoBehaviour {

    [SerializeField] private int[] lives;
    [SerializeField] private int damage = 1;
    private int _currentLives;
    private TextMeshProUGUI _livesText;
    private LevelController _levelController;

    private void Start() {
        var difficultyIndex = PlayerPrefsController.GetDifficulty() - 1;
        _currentLives = lives[difficultyIndex];
        _livesText = GetComponent<TextMeshProUGUI>();
        _levelController = FindObjectOfType<LevelController>();
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        _livesText.text = _currentLives.ToString();
    }

    public void TakeLife() {
        _currentLives -= damage;
        UpdateDisplay();

        if (_currentLives <= 0) {
            StartCoroutine(_levelController.HandleLoseCondition());
        }
    }
}

