using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class BossPoisonProjectile : MonoBehaviour {

    [SerializeField] private MMFeedbacks bossProjectileHitFeedback;
    [SerializeField] private AnimationCurve projectileAcceleration;
    [SerializeField] private LayerMask defenderLayer;
    [SerializeField] private float jumpPower;
    private float _projectileSpeed;
    private int _damage;
    private int _ticks;
    private float _area;
    private Vector2 _targetPos;

    public void GetTarget(Vector2 targetPos) {
        _targetPos = targetPos;
        MoveTowardsTarget();
    }

    public void SetProjectileSpeed(float projectileSpeed) {
        _projectileSpeed = projectileSpeed;
    }
    
    public void SetDamage(int damage) {
        _damage = damage;
    }
    
    public void SetDamageArea(float damageArea) {
        _area = damageArea;
    }
    
    public void SetTicks(int ticks) {
        _ticks = ticks;
    }

    private void MoveTowardsTarget() {
        transform.DOJump(_targetPos, jumpPower, 1, _projectileSpeed)
            .SetSpeedBased(true)
            .SetEase(projectileAcceleration)
            .OnComplete(DamageArea) ;
    }

    private void DamageArea() {
        var enemiesToDamage = Physics2D.OverlapCircleAll(_targetPos, _area, defenderLayer);
        foreach (var enemy in enemiesToDamage) {
            enemy.GetComponent<Health>().Poison(_damage, _ticks);
        }
        bossProjectileHitFeedback.PlayFeedbacks();
        transform.DOKill();
        Destroy(gameObject);
    }
}