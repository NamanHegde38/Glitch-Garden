using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Lobber : MonoBehaviour {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject gun;
    [SerializeField] private int projectileDamage = 25;
    [SerializeField] private float jumpPower = 1;
    [SerializeField] private float tweenTime = 2;
    [SerializeField] private AnimationCurve tweenEase;

    [SerializeField] private LayerMask attackerLayer;

    private AttackerSpawner _myLaneSpawner;
    private Animator _animator;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private GameObject _projectileParent;
    private float _attackerSpeed;
    private TweenCallback _killTween;

    private GameObject _closestEnemy;

    private const string Projectiles = "Projectiles";

    private void Start() {
        SetLaneSpawner();
        CreateProjectileParent();
        DOTween.Init();
        _animator = GetComponent<Animator>();
    }
    
    private void CreateProjectileParent() {
        _projectileParent = GameObject.Find(Projectiles);
        if (!_projectileParent) {
            _projectileParent = new GameObject(Projectiles);
        }
    }
    
    private void Update() {
        _animator.SetBool(IsAttacking, IsAttackerInLane());
    }

    private void SetLaneSpawner() {
        var spawners = FindObjectsOfType<AttackerSpawner>();

        foreach (var spawner in spawners) {
            var isCloseEnough = Mathf.Abs(spawner.transform.position.y - transform.position.y) <= Mathf.Epsilon;
            if (isCloseEnough) {
                _myLaneSpawner = spawner;
            }
        }
    }

    private bool IsAttackerInLane() {
        //return _myLaneSpawner.transform.childCount > 0;

        if (_myLaneSpawner.transform.childCount > 0) {
            if (_myLaneSpawner.transform.GetChild(0).GetComponent<Health>().GetHealth() <= 0) return false;
            _closestEnemy = _myLaneSpawner.transform.GetChild(0).gameObject;
            return true;

        }

        var ray = Physics2D.Raycast(gun.transform.position, Vector2.right, 8f, attackerLayer);
        if (!ray) return false;
        _closestEnemy = ray.transform.gameObject;
        return true;
    }

    public void Fire() {
        if (!IsAttackerInLane()) return;
        //var rayPos = new Vector2(transform.position.x, transform.position.y - 0.25f);
        //var closestEnemy = Physics2D.Raycast(rayPos, Vector2.right, 8f, attackerLayer).collider.gameObject;
        
        var projectile = Instantiate(projectilePrefab, gun.transform.position, transform.rotation);
        
        projectile.transform.parent = _projectileParent.transform;
        projectile.GetComponent<Projectile>().SetDamage(projectileDamage);
        
        //projectile.GetComponent<Rigidbody2D>().velocity = CalculateVelocity(closestEnemy.transform);

        var position = _closestEnemy.transform.position;
        var predictedPosition = new Vector2(position.x, position.y - 0.2f);
        
        if (_closestEnemy.GetComponent<Attacker>() != null) {
            predictedPosition =
                new Vector2(
                    position.x - DistanceTravelled(_closestEnemy.GetComponent<Attacker>().GetMovementSpeed()),
                    position.y - 0.2f);
        }
        
        if (!projectile) return;
        projectile.transform.DOJump(predictedPosition, jumpPower, 1, tweenTime)
            .SetEase(tweenEase)
            .OnComplete(() => KillTween(projectile)) ;
    }

    private void KillTween(GameObject projectile) {
        if (projectile) {
            projectile.transform.DOKill();
            projectile.GetComponent<Projectile>().PlayHitVFX();
            Destroy(projectile);
        }
    }

    private float DistanceTravelled(float speed) {
        var time = tweenTime;
        var distance = speed * time;
        return distance;
    }
    
    /*private Vector3 CalculateVelocity(Transform target) {
        var direction = target.position - gun.transform.position;
        var height = direction.y;
        direction.y = 0;
        var distance = direction.magnitude;
        direction.y = distance;
        distance += height;
        var vel = Mathf.Sqrt(distance * Physics.gravity.magnitude);
        return vel * direction.normalized;
    }*/


}
