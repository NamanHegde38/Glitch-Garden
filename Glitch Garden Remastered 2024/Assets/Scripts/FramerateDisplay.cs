using System.Linq;
using TMPro;
using UnityEngine;

public class FramerateDisplay : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI displayText;
    
    private int _lastFrameIndex;
    private float[] _frameDeltaTimeArray;
    
    private void Awake() {
        _frameDeltaTimeArray = new float[50];
        Singleton();
    }

    private void Singleton() {
        var numberOfObjects = FindObjectsOfType<FramerateDisplay>().Length;
        if (numberOfObjects > 1) {
            DestroyObject();
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        
        PlayerPrefs.SetInt("Levels Unlocked", 1);
        if (PlayerPrefsController.GetShowFPS() == 0) {
            gameObject.SetActive(false);
        }
        
        InvokeRepeating(nameof(SetFramerateDisplay), 1f, 1f);
    }

    private void Update() {
        _frameDeltaTimeArray[_lastFrameIndex] = Time.unscaledDeltaTime;
        _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimeArray.Length;
    }

    private void SetFramerateDisplay() {
        displayText.text = Mathf.RoundToInt(CalculateFPS()).ToString();
    }
    
    private float CalculateFPS() {
        var total = _frameDeltaTimeArray.Sum();
        return Mathf.Clamp(_frameDeltaTimeArray.Length / total, 0f, 144f);
    }

    public void DestroyObject() {
        Destroy(gameObject);
    }
}
