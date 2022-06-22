using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour {

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float defaultVolume;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private int defaultDifficulty;
    
    private LevelLoader _levelLoader;

    private void Start() {
        volumeSlider.value = PlayerPrefsController.GetMasterVolume();
        difficultySlider.value = PlayerPrefsController.GetDifficulty();
        _levelLoader = FindObjectOfType<LevelLoader>();
    }

    public void ChangeVolume() {
        MMSoundManagerTrackEvent.Trigger(
            MMSoundManagerTrackEventTypes.SetVolumeTrack, MMSoundManager.MMSoundManagerTracks.Music, volumeSlider.value);
    }
    
    public void SetDefaults() {
        volumeSlider.value = defaultVolume;
        difficultySlider.value = defaultDifficulty;
    }

    public void SaveAndExit() {
        PlayerPrefsController.SetMasterVolume(volumeSlider.value);
        Debug.Log(volumeSlider.value);
        PlayerPrefsController.SetDifficulty(Mathf.RoundToInt(difficultySlider.value));
        Debug.Log(difficultySlider.value);
        _levelLoader.LoadMainMenu();
    }
}
