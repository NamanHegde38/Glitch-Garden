using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField] private float tweenTime = 0.2f;
    [SerializeField] private Ease tweenEase = Ease.OutSine;
    
    private Transform _owner;
    private Health _health;
    private Slider _healthBar;

    private float _maxHealth;

    private CanvasGroup _healthBarGroup;

    private void Start() {
        _health = _owner.GetComponent<Health>();
        _healthBar = GetComponentInChildren<Slider>();
        _healthBarGroup = GetComponent<CanvasGroup>();
        
        SetMaxHealth();
        _healthBar.maxValue = _maxHealth;

        _health.OnDamageDealt += DealDamage;
    }

    private void SetMaxHealth() {
        _maxHealth = _health.GetHealth();
        _healthBar.value = _maxHealth;
        _healthBarGroup.alpha = 0;
    }
    
    public void GetHealthOwner(Transform owner) {
        _owner = owner;
    }

    public void FadeOutHealthBar() {
        _healthBarGroup.DOFade(0, tweenTime).SetEase(tweenEase);
        Destroy(gameObject, tweenTime + 0.1f);
    }

    private void DealDamage(object sender, Health.OnDamageDealtEventArgs args) {
        if (_healthBarGroup.alpha == 0) {
            _healthBarGroup.DOFade(1, tweenTime).SetEase(tweenEase);
        }
        _healthBar.value = _health.GetHealth();
    }

    private void Update() {
        if (!_owner) return;
        var position = _owner.position;
        transform.position = new Vector2(position.x + _health.GetOffset().x, position.y + _health.GetOffset().y);
    }
}
