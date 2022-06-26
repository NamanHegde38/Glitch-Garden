using System;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Defender : MonoBehaviour {

    [SerializeField] private int starCost = 100;
    [SerializeField] private DefenderMaterial material;
    [SerializeField] private DefenderType type;

    [SerializeField] private MMFeedbacks spawnFeedback;
    [SerializeField] private MMFeedbacks starCollectFeedback;

    [SerializeField] private float tweenTime = 0.2f;
    [SerializeField] private Ease tweenEase = Ease.InQuad;

    private Health _health;
    
    private void Start() {
        PlaySpawnFeedback();
        _health = GetComponent<Health>();
    }

    private void PlaySpawnFeedback() {
        spawnFeedback.Initialization();
        spawnFeedback.PlayFeedbacks();
    }

    private void Update() {
        if (transform.localPosition.x > 0) return;
        _health.DealDamage(5000);
    }

    public void AddStars(int amount) {
        FindObjectOfType<StarDisplay>().AddStars(amount);
        if (starCollectFeedback) {
            starCollectFeedback.PlayFeedbacks();
        }
    }

    public int GetStarCost() {
        return starCost;
    }

    public DefenderMaterial GetDefenderMaterial() {
        return material;
    }

    public DefenderType GetDefenderType() {
        return type;
    }

    public void MoveDefenderX(float moveAmount) {
        transform.DOLocalMoveX(transform.localPosition.x - moveAmount, tweenTime).SetEase(tweenEase);
    }
}

