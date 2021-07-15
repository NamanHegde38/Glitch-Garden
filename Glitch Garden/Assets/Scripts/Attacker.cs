using System;
using UnityEngine;

public class Attacker : MonoBehaviour {

    private float _currentSpeed;
    private GameObject _currentTarget;
    private Animator _anim;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private LevelController _levelController;

    private int _difficulty = 1;
    
    private void Awake() {
        _levelController = FindObjectOfType<LevelController>();
        
        if (!_levelController) return;
        _levelController.AttackerSpawned();
    }

    private void Start() {
        _difficulty = PlayerPrefsController.GetDifficulty();
        _anim = GetComponent<Animator>();
    }

    private void Update() {
        transform.Translate(Vector2.left * (_currentSpeed * Time.deltaTime));
        UpdateAnimationState();
    }

    private void UpdateAnimationState() {
        if (!_currentTarget) {
            _anim.SetBool(IsAttacking, false);
        }
    }

    public void SetMovementSpeed(float speed) {
        _currentSpeed = speed;
    }

    public void Attack(GameObject target) {
        _anim.SetBool(IsAttacking, true);
        _currentTarget = target;
    }

    public void StrikeCurrentTarget(float damage) {
        if (!_currentTarget) return;
        
        var health = _currentTarget.GetComponent<Health>();
        
        if (!health) return;
        switch (_difficulty) {
            case 1:
                damage *= 1f;
                break;
            case 2:
                damage *= 1.25f;
                break;
            case 3:
                damage *= 1.5f;
                break;
        }
        health.DealDamage(Mathf.RoundToInt(damage));
    }

    private void OnDestroy() {
        if (!_levelController) return;
        _levelController.AttackerKilled();
    }
}
