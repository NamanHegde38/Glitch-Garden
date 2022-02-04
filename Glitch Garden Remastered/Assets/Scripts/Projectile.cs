using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Projectile : MonoBehaviour {
    
    private int _damage = 25;
    private Vector2 _startPos;
    private BoxCollider2D _collider;

    private bool _isShotStraight;
    private float _projectileSpeed;

    [SerializeField] private int criticalPercentage = 20;
    [SerializeField] private float criticalMultiplier = 2f;
    [SerializeField] private MMFeedbacks projectileFeedback;
    [SerializeField] private MMFeedbacks criticalHitFeedback;

    private void Start() {
        _startPos = transform.position;
        _collider = GetComponent<BoxCollider2D>();
    }

    public void SetShotStraight(bool isShotStraight, float projectileSpeed = 2.5f) {
        _isShotStraight = isShotStraight;
        _projectileSpeed = projectileSpeed;
    }
    
    private void Update() {
        _collider.enabled = Mathf.RoundToInt(transform.position.y) == Mathf.RoundToInt(_startPos.y);

        if (_isShotStraight) {
            transform.Translate(Vector2.right * (_projectileSpeed * Time.deltaTime));
        }
    }

    public void SetDamage(int damage) {
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var health = other.GetComponent<Health>();
        var attacker = other.GetComponent<Attacker>();

        if (!attacker || !health) return;
        if (Random.Range(1, 101) < criticalPercentage) {
            health.DealDamage(Mathf.RoundToInt(_damage * criticalMultiplier));
            criticalHitFeedback.PlayFeedbacks();
        }
        else {
            health.DealDamage(_damage);
        }
        projectileFeedback.PlayFeedbacks();
        transform.DOKill();
        Destroy(gameObject);
    }

    public void PlayHitVFX() {
        projectileFeedback.PlayFeedbacks();
    }
}
