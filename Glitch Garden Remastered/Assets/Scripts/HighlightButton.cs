using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HighlightButton : MonoBehaviour {

    [SerializeField] private float tweenTime = 0.2f;
    [SerializeField] private Ease tweenEase = Ease.Linear;
    private Image _image;

    private void Start() {
        _image = GetComponent<Image>();
    }

    public void EnableHighlight() {
        _image.DOFade(1, tweenTime).SetEase(tweenEase);
    }
    
    public void DisableHighlight() {
        _image.DOFade(0, tweenTime).SetEase(tweenEase);
    }
}
