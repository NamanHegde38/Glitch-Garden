using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    
    private int _levelNumber;
    private Button _button;

    private void Start() {
        _button = GetComponent<Button>();
        _levelNumber = int.Parse(transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);

        if (PlayerPrefsController.GetLevelsUnlocked() < _levelNumber) {
            //_button.interactable = false;
        }
        
    }
}
