using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour {

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float defaultVolume;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private int defaultDifficulty;

    private MusicPlayer _musicPlayer;
    private LevelLoader _levelLoader;

    private void Start() {
        volumeSlider.value = PlayerPrefsController.GetMasterVolume();
        difficultySlider.value = PlayerPrefsController.GetDifficulty();
        _musicPlayer = FindObjectOfType<MusicPlayer>();
        _levelLoader = FindObjectOfType<LevelLoader>();
    }

    public void ChangeVolume() {
        if (_musicPlayer) {
            _musicPlayer.SetVolume(volumeSlider.value);
        }
    }
    
    public void SetDefaults() {
        volumeSlider.value = defaultVolume;
        difficultySlider.value = defaultDifficulty;
    }

    public void SaveAndExit() {
        PlayerPrefsController.SetMasterVolume(volumeSlider.value);
        PlayerPrefsController.SetDifficulty(Mathf.RoundToInt(difficultySlider.value));
        _levelLoader.LoadMainMenu();
    }
}
