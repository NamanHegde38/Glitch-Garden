using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour {

    [SerializeField] private float tweenTime = 0.2f;
    [SerializeField] private Ease tweenEase = Ease.OutQuad;
    
    private Health _bossHealth;
    private Slider _healthBar;

    private void Start() {
        _healthBar = GetComponent<Slider>();
        _bossHealth = FindObjectOfType<Boss>().GetComponent<Health>();
        SetMaxHealth();

        _bossHealth.OnDamageDealt += DealDamage;
    }

    private void SetMaxHealth() {
        _healthBar.maxValue = _bossHealth.GetHealth();
        _healthBar.value = _bossHealth.GetHealth();
    }

    private void DealDamage(object sender, Health.OnDamageDealtEventArgs args) {
        _healthBar.DOValue(args.Damage, tweenTime).SetEase(tweenEase);
    }
}
