using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

    [SerializeField] private Button[] levelButtonArray;

    private void Start() {
        if (PlayerPrefsController.GetUnlockLevels() == 0) {
            var levelsUnlocked = PlayerPrefsController.GetLevelsUnlocked();
            for (var i = 0; i < levelsUnlocked; i++) {
                levelButtonArray[i].interactable = true;
            }
        }
        else {
            for (var i = 0; i < levelButtonArray.Length; i++) {
                levelButtonArray[i].interactable = true;
            }
        }
    }
}
