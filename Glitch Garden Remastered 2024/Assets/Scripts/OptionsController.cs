using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour {

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float defaultVolume;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private int defaultDifficulty;
    
    [SerializeField] private TextMeshProUGUI framerateText;
    [SerializeField] private Toggle showFPSToggle;
    [SerializeField] private Toggle unlockLevelsToggle;

    private int _framerateLimit;
    private MusicPlayer _musicPlayer;
    private LevelLoader _levelLoader;

    private void Start() {
        volumeSlider.value = PlayerPrefsController.GetMasterVolume() * 10;
        difficultySlider.value = PlayerPrefsController.GetDifficulty();
        
        SetFramerateLimit(PlayerPrefsController.GetFramerateLimit());
        showFPSToggle.isOn = PlayerPrefsController.GetShowFPS() == 1;
        unlockLevelsToggle.isOn = PlayerPrefsController.GetUnlockLevels() == 1;

        _musicPlayer = FindObjectOfType<MusicPlayer>();
        _levelLoader = FindObjectOfType<LevelLoader>();
    }

    public void ChangeVolume() {
        if (_musicPlayer) {
            _musicPlayer.SetVolume(volumeSlider.value / 10);
        }
    }

    public void RaiseFramerateLimit() {
        switch (_framerateLimit) {
            case >= 144:
                return;
            case 60:
                SetFramerateLimit(144);
                break;
            case 30:
                SetFramerateLimit(60);
                break;
        }
    }

    public void LowerFramerateLimit() {
        switch (_framerateLimit) {
            case <= 30:
                return;
            case 60:
                SetFramerateLimit(30);
                break;
            case 144:
                SetFramerateLimit(60);
                break;
        }
    }
    
    public void SetDefaults() {
        volumeSlider.value = defaultVolume * 10;
        difficultySlider.value = defaultDifficulty;
        
        SetFramerateLimit(60);
        showFPSToggle.isOn = false;
        unlockLevelsToggle.isOn = false;
    }

    public void SaveAndExit() {
        PlayerPrefsController.SetMasterVolume(volumeSlider.value / 10);
        PlayerPrefsController.SetDifficulty(Mathf.RoundToInt(difficultySlider.value));
        
        PlayerPrefsController.SetFramerateLimit(_framerateLimit);
        PlayerPrefsController.SetShowFPS(showFPSToggle.isOn ? 1 : 0);
        PlayerPrefsController.SetUnlockLevels(unlockLevelsToggle.isOn ? 1 : 0);
        
        FindObjectOfType<FramerateLimiter>()?.SetFramerateLimit();
        FindObjectOfType<FramerateDisplay>(true)?.gameObject.SetActive(showFPSToggle.isOn);
        _levelLoader.LoadMainMenu();
    }

    private void SetFramerateLimit(int limit) {
        _framerateLimit = limit;
        framerateText.text = _framerateLimit.ToString();
    }
}
