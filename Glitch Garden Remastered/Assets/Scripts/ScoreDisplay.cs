using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour {
    
    private int _score;
    
    [SerializeField] private int scoreOverTime = 1000;
    [SerializeField] private int scoreTime = 60;
    [SerializeField] private bool giveScoreOverTime = true;

    private TextMeshProUGUI _scoreText;

    private IEnumerator Start() {
        _scoreText = GetComponent<TextMeshProUGUI>();
        UpdateDisplay();

        while (giveScoreOverTime) {
            yield return new WaitForSeconds(scoreTime);
            AddScore(scoreOverTime);
        }
    }

    private void UpdateDisplay() {
        _scoreText.text = _score.ToString();
    }

    public void AddScore(int amount) {
        _score += amount;
        UpdateDisplay();
    }

    public int GetScore() {
        return _score;
    }
}
