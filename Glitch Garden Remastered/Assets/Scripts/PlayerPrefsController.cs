using UnityEngine;

public class PlayerPrefsController : MonoBehaviour {

    private const string MasterVolumeKey = "Master Volume";
    private const string DifficultyKey = "Difficulty";
    private const string LevelsUnlockedKey = "Levels Unlocked";

    private const float MinVolume = 0f;
    private const float MaxVolume = 1f;

    private const int MinDifficulty = 1;
    private const int MaxDifficulty = 3;

    public static void SetMasterVolume(float volume) {
        if (volume >= MinVolume && volume <= MaxVolume) {
            Debug.Log("Master volume set to " + volume);
            PlayerPrefs.SetFloat(MasterVolumeKey, volume);
            PlayerPrefs.Save();
        }
        else {
            Debug.LogError("Master volume is out of range");
        }
    }

    public static float GetMasterVolume() {
        return PlayerPrefs.GetFloat(MasterVolumeKey, 0.5f);
    }
    
    public static void SetDifficulty(int difficulty) {
        if (difficulty >= MinDifficulty && difficulty <= MaxDifficulty) {
            Debug.Log("Difficulty Set to " + difficulty);
            PlayerPrefs.SetInt(DifficultyKey, difficulty);
            PlayerPrefs.Save();
        }
        else {
            Debug.LogError("Difficulty is out of range");
        }
    }

    public static int GetDifficulty() {
        return PlayerPrefs.GetInt(DifficultyKey, 1);
    }

    public static void UnlockLevel(int level) {
        if (level <= GetLevelsUnlocked()) return;
        
        Debug.Log("Unlocked Level " + level);
        PlayerPrefs.SetInt(LevelsUnlockedKey, level);
        PlayerPrefs.Save();
    }

    public static int GetLevelsUnlocked() {
        return PlayerPrefs.GetInt(LevelsUnlockedKey, 1);
    }
}
