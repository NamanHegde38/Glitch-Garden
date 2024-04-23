using UnityEngine;

public class FramerateLimiter : MonoBehaviour {
    
    private void Awake() {
        Singleton();
    }

    private void Singleton() {
        var numberOfObjects = FindObjectsOfType<FramerateLimiter>().Length;
        if (numberOfObjects > 1) {
            DestroyObject();
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        SetFramerateLimit();
    }

    public void SetFramerateLimit() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefsController.GetFramerateLimit() - 1;
    }
    
    public void DestroyObject() {
        Destroy(gameObject);
    }
}
