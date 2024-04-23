using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class BossProjectile : MonoBehaviour {

    [SerializeField] private MMFeedbacks bossProjectileHitFeedback;
    [SerializeField] private Ease projectileAcceleration;
    [SerializeField] private LayerMask defenderLayer;
    private float _projectileSpeed;
    private int _damage;
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

    private void MoveTowardsTarget() {
        transform.DOMove(_targetPos, _projectileSpeed)
            .SetSpeedBased(true)
            .SetEase(projectileAcceleration)
            .OnComplete(DamageArea);
    }

    private void DamageArea() {
        var enemiesToDamage = Physics2D.OverlapCircleAll(_targetPos, _area, defenderLayer);
        foreach (var enemy in enemiesToDamage) {
            enemy.GetComponent<Health>().DealDamage(_damage);
        }
        bossProjectileHitFeedback.PlayFeedbacks();
        transform.DOKill();
        Destroy(gameObject);
    }
}
