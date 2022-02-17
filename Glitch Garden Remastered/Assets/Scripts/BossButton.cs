using UnityEngine;
using UnityEngine.UI;

public class BossButton : MonoBehaviour {

    [SerializeField] private int requiredLevel;
    [SerializeField] private bool requiresFirstBoss;
    [SerializeField] private bool requiresSecondBoss;
    [SerializeField] private bool requiresThirdBoss;
    [SerializeField] private bool requiresFourthBoss;
    [SerializeField] private bool requiresFifthBoss;

    [SerializeField] private bool hasTooltip;

    private Button _button;
    
    private void Start() {
        _button = GetComponent<Button>();
        _button.interactable = true;
        if (hasTooltip) {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
        if (PlayerPrefsController.GetLevelsUnlocked() < requiredLevel) {
            SetButtonInteractivity();
            return;
        }
        if (requiresFirstBoss && PlayerPrefsController.GetFirstBossDefeated() != 1) {
            SetButtonInteractivity();
            return;
        }
        if (requiresSecondBoss && PlayerPrefsController.GetSecondBossDefeated() != 1) {
            SetButtonInteractivity();
            return;
        }
        if (requiresThirdBoss && PlayerPrefsController.GetThirdBossDefeated() != 1) {
            SetButtonInteractivity();
            return;
        }
        if (requiresFourthBoss && PlayerPrefsController.GetFourthBossDefeated() != 1) {
            SetButtonInteractivity();
            return;
        }
        if (requiresFifthBoss && PlayerPrefsController.GetFifthBossDefeated() != 1) {
            SetButtonInteractivity();
        }
    }

    private void SetButtonInteractivity() {
        _button.interactable = false;
        if (hasTooltip) {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
