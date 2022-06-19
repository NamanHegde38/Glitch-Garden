using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour {

    private TextMeshProUGUI _text;
    
    private void Start() {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = PlayerPrefsController.GetHighScore().ToString();
    }
}
