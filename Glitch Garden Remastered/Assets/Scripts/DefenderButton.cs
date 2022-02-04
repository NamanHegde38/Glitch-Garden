using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using TMPro;

public class DefenderButton : MonoBehaviour {

    [SerializeField] private Defender defenderPrefab;
    [SerializeField] private float tweenTime = .2f;
    [SerializeField] private Ease tweenEase;
    [SerializeField] private MMFeedbacks selectFeedback;
    
    private SpriteRenderer _sprite;
    private Material _shader;
    private DefenderButton[] _buttons;
    private DefenderSpawner _spawner;
    private bool _buttonPressed = false;
    private static readonly int OutlineAlpha = Shader.PropertyToID("_OutlineAlpha");

    private void Start() {
        DOTween.Init();
        _sprite = GetComponent<SpriteRenderer>();
        _shader = _sprite.material;
        _buttons = FindObjectsOfType<DefenderButton>();
        _spawner = FindObjectOfType<DefenderSpawner>();

        LabelButtonCost();
    }

    private void LabelButtonCost() {
        var costText = GetComponentInChildren<TextMeshProUGUI>();
        if (!costText) {
            Debug.LogError(name + " has no cost text");
        }
        else {
            costText.text = defenderPrefab.GetStarCost().ToString();
        }
    }

    private void OnMouseDown() {
        if (_buttonPressed) {
            _sprite.DOColor(new Color32(64, 64, 64, 255), tweenTime).SetEase(tweenEase);
            _shader.DOFloat(0, OutlineAlpha, tweenTime).SetEase(tweenEase);
            _spawner.SetSelectedDefender(null);
            _buttonPressed = false;
        }
        else {
            foreach (var button in _buttons) {
                button.GetComponent<SpriteRenderer>().DOColor(new Color32(64, 64, 64, 255), tweenTime).SetEase(tweenEase);
                button.GetComponent<SpriteRenderer>().material.DOFloat(0, OutlineAlpha, tweenTime).SetEase(tweenEase);
                button.SetButtonPressed(false);
            }
        
            _sprite.DOColor(Color.white, tweenTime).SetEase(tweenEase);
            _shader.DOFloat(1, OutlineAlpha, tweenTime).SetEase(tweenEase);
            selectFeedback.PlayFeedbacks();
            _spawner.SetSelectedDefender(defenderPrefab);
            _buttonPressed = true;
        }
    }

    public void SetButtonPressed(bool buttonPressed) {
        _buttonPressed = buttonPressed;
    }
}
