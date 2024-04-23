using UnityEngine;

public class PlayerPrefsController : MonoBehaviour {

    private const string MasterVolumeKey = "Master Volume";
    private const string DifficultyKey = "Difficulty";
    private const string LevelsUnlockedKey = "Levels Unlocked";
    private const string HighScoreKey = "High Score";

    private const string FramerateLimitKey = "Framerate Limit";
    private const string ShowFPSKey = "Show FPS";
    private const string UnlockLevelsKey = "Unlock All Levels";

    private const string BeatFirstBossKey = "Beat First Boss";
    private const string BeatSecondBossKey = "Beat Second Boss";
    private const string BeatThirdBossKey = "Beat Third Boss";
    private const string BeatFourthBossKey = "Beat Fourth Boss";
    private const string BeatFifthBossKey = "Beat Fifth Boss";

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
    
    public static void SetHighScore(int score) {
        if (PlayerPrefs.GetInt(HighScoreKey) < score) {
            Debug.Log("High Score set to " + score);
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }
        else {
            Debug.LogError("High Score is out of range");
        }
    }
    
    public static int GetHighScore() {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public static void SetFramerateLimit(int limit) {
        if (limit is 30 or 60 or 144) {
            PlayerPrefs.SetInt(FramerateLimitKey, limit);
            Debug.Log("Framerate Limit set to " + limit);
            PlayerPrefs.Save();
        }
        else {
            Debug.LogError("Framerate Limit is out of range");
        }
    }

    public static int GetFramerateLimit() {
        return PlayerPrefs.GetInt(FramerateLimitKey, 60);
    }
    
    public static void SetShowFPS(int toggle) {
        if (toggle is 0 or 1) {
            PlayerPrefs.SetInt(ShowFPSKey, toggle);
            Debug.Log("Show FPS set to " + (toggle == 1 ? "true" : "false"));
            PlayerPrefs.Save();
        }
        else {
            Debug.LogError("Show FPS toggle is out of range");
        }
    }

    public static int GetShowFPS() {
        return PlayerPrefs.GetInt(ShowFPSKey, 0);
    }
    
    public static void SetUnlockLevels(int toggle) {
        if (toggle is 0 or 1) {
            PlayerPrefs.SetInt(UnlockLevelsKey, toggle);
            Debug.Log("Unlock All Levels set to " + (toggle == 1 ? "true" : "false"));
            PlayerPrefs.Save();
        }
        else {
            Debug.LogError("Unlocked Levels Toggle is out of range");
        }
    }

    public static int GetUnlockLevels() {
        return PlayerPrefs.GetInt(UnlockLevelsKey, 0);
    }
    
    public static void FirstBossDefeated() {
        PlayerPrefs.SetInt(BeatFirstBossKey, 1);
        PlayerPrefs.Save();
    }
    
    public static void SecondBossDefeated() {
        PlayerPrefs.SetInt(BeatSecondBossKey, 1);
        PlayerPrefs.Save();
    }
    
    public static void ThirdBossDefeated() {
        PlayerPrefs.SetInt(BeatThirdBossKey, 1);
        PlayerPrefs.Save();
    }
    
    public static void FourthBossDefeated() {
        PlayerPrefs.SetInt(BeatFourthBossKey, 1);
        PlayerPrefs.Save();
    }
    
    public static void FifthBossDefeated() {
        PlayerPrefs.SetInt(BeatFifthBossKey, 1);
        PlayerPrefs.Save();
    }
    
    public static int GetFirstBossDefeated() {
        return PlayerPrefs.GetInt(BeatFirstBossKey, 0);
    }
    
    public static int GetSecondBossDefeated() {
        return PlayerPrefs.GetInt(BeatSecondBossKey, 0);
    }
    
    public static int GetThirdBossDefeated() {
        return PlayerPrefs.GetInt(BeatThirdBossKey, 0);
    }
    
    public static int GetFourthBossDefeated() {
        return PlayerPrefs.GetInt(BeatFourthBossKey, 0);
    }
    
    public static int GetFifthBossDefeated() {
        return PlayerPrefs.GetInt(BeatFifthBossKey, 0);
    }
    
    public static void UnlockLevel(int level) {
        if (level <= GetLevelsUnlocked()) return;
        if (level > 60) return;
        
        Debug.Log("Unlocked Level " + level);
        PlayerPrefs.SetInt(LevelsUnlockedKey, level);
        PlayerPrefs.Save();
    }

    public static int GetLevelsUnlocked() {
        return PlayerPrefs.GetInt(LevelsUnlockedKey, 1);
    }
}
