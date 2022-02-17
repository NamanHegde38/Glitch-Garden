using System;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Health : MonoBehaviour {
    
    [Header("Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private GameObject deathVFX;

    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks hurtFeedback;
    [SerializeField] private MMFeedbacks deathFeedback;
    
    private Animator _animator;

    public event EventHandler<OnDamageDealtEventArgs> OnDamageDealt;

    public class OnDamageDealtEventArgs : EventArgs {
        public int Damage;
    }

    private int _difficulty = 1;
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Death = Animator.StringToHash("Death");

    private void Start() {
        _difficulty = PlayerPrefsController.GetDifficulty();
        _animator = GetComponent<Animator>();
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
        
        if (deathFeedback) {
            deathFeedback?.PlayFeedbacks();
        }
        
        if (GetComponent<Attacker>()) {
            _animator.SetTrigger(Death);
        }
        else if (GetComponent<Defender>()) {
            DestroyObject();
        }
    }

    public void DestroyObject() {
        Destroy(gameObject);
    }
    
    private void TriggerDeathVFX() {
        if (!deathVFX) return;
        var deathVFXObject = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(deathVFXObject, 1f);
    }
}
