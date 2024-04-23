using UnityEngine;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour {

    [SerializeField] private int requiredLevel;
    private Button _button;

    private void Start() {
        _button = GetComponent<Button>();
        
        if (PlayerPrefsController.GetLevelsUnlocked() >= requiredLevel) {
            _button.interactable = true;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else {
            _button.interactable = false;
        }
    }
}
