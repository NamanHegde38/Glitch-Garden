using TMPro;
using DG.Tweening;
using UnityEngine;

public class Tooltip : MonoBehaviour {

    [SerializeField] private string tooltipText;
    [SerializeField] private Vector2 padding = new Vector2(8, 8);
    [SerializeField] private float tweenTime = 0.5f;
    [SerializeField] private Ease tweenEase = Ease.Linear;
    
    private RectTransform _backgroundRectTransform;
    private TextMeshProUGUI _textMeshPro;
    private CanvasGroup _canvasGroup;

    private void Awake() {
        DOTween.Init();
        _canvasGroup = GetComponent<CanvasGroup>();
        _backgroundRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        _textMeshPro = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        SetText();
        HideTooltip();
    }

    private void SetText() {
        _textMeshPro.SetText(tooltipText);
        _textMeshPro.ForceMeshUpdate();
        
        var textSize = _textMeshPro.GetRenderedValues(false);
        _backgroundRectTransform.sizeDelta = textSize + padding;
    }

    public void ShowTooltip() {
        _canvasGroup.DOFade(1, tweenTime).SetEase(tweenEase);
    }

    public void HideTooltip() {
        _canvasGroup.DOFade(0, tweenTime).SetEase(tweenEase);
    }
}
