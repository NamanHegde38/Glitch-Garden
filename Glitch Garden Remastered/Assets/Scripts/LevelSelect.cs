using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

    [SerializeField] private Button[] levelButtonArray;

    private void Start() {
        var levelsUnlocked = PlayerPrefsController.GetLevelsUnlocked();

        for (var i = 0; i < levelsUnlocked; i++) {
            levelButtonArray[i].interactable = true;
        }
    }
}
