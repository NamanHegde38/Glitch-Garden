using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelSelectManager : MonoBehaviour {

    [SerializeField] private RectTransform seasonsGrid;
    [SerializeField] private float amountToMove = 1600f;
    [SerializeField] private int numberOfSeasons = 4;
    [SerializeField] private Image secondaryImage;
    [SerializeField] private Sprite[] seasonImage;
    [SerializeField] private float tweenTime = 0.5f;
    [SerializeField] private Ease shiftEase = Ease.OutQuad;
    [SerializeField] private Ease fadeEase = Ease.OutQuad;

    private int _seasonNumber;
    private Image _imageRenderer;

    private void Start() {
        _imageRenderer = GetComponent<Image>();
    }

    public void MoveLeft() {
        if (_seasonNumber <= 0) return;
        if (DOTween.IsTweening("Move Left", true)) return;
        if (DOTween.IsTweening("Move Right", true)) return;
        seasonsGrid.DOLocalMoveX(seasonsGrid.localPosition.x + amountToMove, tweenTime).SetEase(shiftEase).SetId("Move Left");
        _seasonNumber--;
        StartCoroutine(ChangeImage());
    }
    
    public void MoveRight() {
        if (_seasonNumber >= numberOfSeasons - 1) return;
        if (DOTween.IsTweening("Move Right", true)) return;
        if (DOTween.IsTweening("Move Left", true)) return;
        seasonsGrid.DOLocalMoveX(seasonsGrid.localPosition.x - amountToMove, tweenTime).SetEase(shiftEase).SetId("Move Right");
        _seasonNumber++;
        StartCoroutine(ChangeImage());
    }

    private IEnumerator ChangeImage() {
        secondaryImage.sprite = seasonImage[_seasonNumber];
        secondaryImage.color = new Color(_imageRenderer.color.r, _imageRenderer.color.g, _imageRenderer.color.b, 1);
        var primaryFade = _imageRenderer.DOFade(0, tweenTime).SetEase(fadeEase);
        yield return primaryFade.WaitForCompletion();
        _imageRenderer.sprite = seasonImage[_seasonNumber];
        _imageRenderer.color = new Color(_imageRenderer.color.r, _imageRenderer.color.g, _imageRenderer.color.b, 1);
        secondaryImage.color = new Color(_imageRenderer.color.r, _imageRenderer.color.g, _imageRenderer.color.b, 0);
    }
}
