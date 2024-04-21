using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine.Pool;

public class Lobber : MonoBehaviour {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject gun;
    [SerializeField] private int projectileDamage = 25;
    [SerializeField] private float jumpPower = 1;
    [SerializeField] private float tweenTime = 2;
    [SerializeField] private AnimationCurve tweenEase;
    [SerializeField] private bool usePool = true;

    [SerializeField] private LayerMask attackerLayer;
    [SerializeField] private MMFeedbacks shootFeedback;

    private AttackerSpawner _myLaneSpawner;
    private Animator _animator;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private GameObject _projectileParent;
    private ObjectPool<GameObject> _objectPool;
    private float _attackerSpeed;
    private TweenCallback _killTween;
    private Coroutine _killTweenCoroutine;

    private GameObject _closestEnemy;

    private const string Projectiles = "Projectiles";

    private void Start() {
        _objectPool = new ObjectPool<GameObject>(() => {
            return Instantiate(projectilePrefab);
        }, projectile => { 
            projectile.gameObject.SetActive(true); 
        }, projectile => { 
            
            projectile.gameObject.SetActive(false);
        }, projectile => { 
            Destroy(projectile.gameObject); 
        }, true, 2, 3);
        
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
        
        var gunPos = gun.transform.position;

        var projectileObject = usePool ? _objectPool.Get() : Instantiate(projectilePrefab);
        projectileObject.transform.position = gunPos;
        projectileObject.transform.rotation = transform.rotation;

        var projectile = projectileObject.GetComponent<Projectile>();
        var projectileRigidbody = projectileObject.GetComponent<Rigidbody2D>();
        
        projectile.ClearTrailRenderers();
        projectile.OnProjectileDestroyed += ReturnProjectileToPool;
        projectileObject.transform.parent = _projectileParent.transform;
        projectile.SetDamage(projectileDamage);

        shootFeedback.PlayFeedbacks(gunPos);

        var position = _closestEnemy.transform.position;
        var predictedPosition = new Vector2(position.x, position.y - 0.2f);
        
        if (_closestEnemy.GetComponent<Attacker>() != null) {
            predictedPosition =
                new Vector2(
                    position.x - DistanceTravelled(_closestEnemy.GetComponent<Attacker>().GetMovementSpeed()),
                    position.y - 0.2f);
        }
        
        if (!projectileObject) return;
        projectileRigidbody.DOJump(predictedPosition, jumpPower, 1, tweenTime)
            .SetEase(tweenEase)
            .OnComplete(() =>
                _killTweenCoroutine = StartCoroutine(KillTween(projectile, projectileRigidbody)));
    }

    private IEnumerator KillTween(Projectile projectile, Rigidbody2D projectileRigidbody) {
        projectileRigidbody.DOKill();
        projectile.PlayHitVFX();
        yield return new WaitForSeconds(0.08f);
        DespawnProjectile(projectile.gameObject);
    }
    
    private void ReturnProjectileToPool(object sender, Projectile.OnProjectileDestroyedEventArgs eventArgs) {
        DespawnProjectile(eventArgs.Projectile);
        eventArgs.Projectile.GetComponent<Projectile>().OnProjectileDestroyed -= ReturnProjectileToPool;
    }

    private void DespawnProjectile(GameObject projectile) {
        

        if (usePool) {
            if (projectile.activeSelf) {
                _objectPool.Release(projectile);
            }
        }
        else {
            Destroy(projectile);
        }
        
    }

    private void OnDisable() {
        if (_killTweenCoroutine != null) {
            StopCoroutine(_killTweenCoroutine);
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
