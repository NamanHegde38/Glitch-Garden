using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Health : MonoBehaviour {
    
    [Header("Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private GameObject deathVFX;

    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks hurtFeedback;
    [SerializeField] private MMFeedbacks deathFeedback;

    [Header("Freeze")]
    [SerializeField] private float freezeSpeed = 0.5f;

    [Header("Health Bar")]
    [SerializeField] private bool hasHealthBar = true;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Vector2 barOffset = new(0, 0.5f);

    private Animator _animator;
    private bool _poisoned;
    private GameObject _healthBar;

    private List<int> _poisonTickTimers = new List<int>();

    public event EventHandler<OnDamageDealtEventArgs> OnDamageDealt;

    public class OnDamageDealtEventArgs : EventArgs {
        public int Damage;
    }

    private int _difficulty = 1;
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Death = Animator.StringToHash("Death");

    private void Awake() {
        var position = transform.position;
        if (!hasHealthBar) return;
        
        _healthBar = Instantiate(healthBarPrefab, new Vector2(position.x + barOffset.x, position.y + barOffset.y), Quaternion.identity);
        _healthBar.transform.SetParent(GameObject.FindGameObjectWithTag("Health Bar Canvas").transform);
        _healthBar.transform.localScale /= 160;
        _healthBar.GetComponent<HealthBar>().GetHealthOwner(transform);
    }

    private void Start() {
        _difficulty = PlayerPrefsController.GetDifficulty();
        _animator = GetComponent<Animator>();
        _animator.speed = 1;
        health = SetHealth(health);
    }

    public int SetHealth(float baseHealth) {
        switch (_difficulty) {
            case 1:
                baseHealth *= 1f;
                break;
            case 2:
                baseHealth *= 1.5f;
                break;
            case 3:
                baseHealth *= 2f;
                break;
        }

        return Mathf.RoundToInt(baseHealth);
    }

    public int GetHealth() {
        return health;
    }
    
    public void DealDamage(int damage) {
        health -= damage;
        
        _animator.SetTrigger(Hurt);
        if (hurtFeedback) {
            hurtFeedback?.PlayFeedbacks();
        }
        
        OnDamageDealt?.Invoke(this, new OnDamageDealtEventArgs{Damage = damage});
        
        if (health > 0) return;
        

        if (GetComponent<Attacker>()) {
            _animator.SetTrigger(Death);
            _healthBar.GetComponent<HealthBar>().FadeOutHealthBar();
            deathFeedback.PlayFeedbacks();
        }
        else if (GetComponent<Boss>()) {
            _animator.SetTrigger(Death);
        }
        else if (GetComponent<Defender>()) {
            DestroyObject();
            _healthBar.GetComponent<HealthBar>().FadeOutHealthBar();
            deathFeedback.PlayFeedbacks();
        }
    }

    public void Freeze(float freezeTime) {
        _animator.speed = freezeSpeed;
        StartCoroutine(Unfreeze(freezeTime));
    }

    private IEnumerator Unfreeze(float freezeTime) {
        yield return new WaitForSeconds(freezeTime);
        _animator.speed = 1;
    }

    public void Poison(int damage, int ticks) {
        if (_poisonTickTimers.Count <= 0) {
            _poisonTickTimers.Add(ticks);
            StartCoroutine(PoisonDamage(damage));
        }
        else {
            _poisonTickTimers.Add(ticks);
        }
    }
    
    private IEnumerator PoisonDamage(int damage) {
        while (_poisonTickTimers.Count > 0) {
            for (var i = 0; i < _poisonTickTimers.Count; i++) {
                _poisonTickTimers[i]--;
            }
            DealDamage(damage);
            _poisonTickTimers.RemoveAll(i => i == 0);
            yield return new WaitForSeconds(0.75f);
        }
    }
    
    public void DestroyObject() {
        transform.DOKill();
        Destroy(gameObject);
    }

    public void PlayDeathFeedbacks() {
        if (deathFeedback) {
            deathFeedback.PlayFeedbacks();
        }
    }

    public Vector2 GetOffset() {
        return barOffset;
    }

    private void TriggerDeathVFX() {
        if (!deathVFX) return;
        var deathVFXObject = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(deathVFXObject, 1f);
    }
}
