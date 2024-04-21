using System;
using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Pool;

public class Shooter : MonoBehaviour {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject gun;
    [SerializeField] private float projectileSpeed = 1f;
    [SerializeField] private int projectileDamage = 25;
    [SerializeField] private bool usePool = true;

    [SerializeField] private LayerMask attackerLayer;
    [SerializeField] private MMFeedbacks shootFeedback;

    private AttackerSpawner _myLaneSpawner;
    private Animator _animator;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    
    private GameObject _projectileParent;
    private ObjectPool<GameObject> _objectPool;


    private const string Projectiles = "Projectiles";
    private void Start() {
        _objectPool = new ObjectPool<GameObject>(() => {
            return Instantiate(projectilePrefab);
        }, projectile => { 
            projectile.gameObject.SetActive(true); 
        }, projectile => { 
            projectile.gameObject.SetActive(false); 
            projectile.transform.position = gun.transform.position;
            projectile.transform.rotation = transform.rotation;
        }, projectile => { 
            Destroy(projectile.gameObject); 
        }, true, 2, 3);
        
        SetLaneSpawner();
        CreateProjectileParent();
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
        
        return _myLaneSpawner.transform.childCount > 0 ? true : Physics2D.Raycast(gun.transform.position, Vector2.right, 8f, attackerLayer);
    }
    
    public void Fire() {
        var gunPos = gun.transform.position;
        
        var projectileSpawn = usePool ? _objectPool.Get() : Instantiate(projectilePrefab);
        projectileSpawn.transform.position = gunPos;
        projectileSpawn.transform.rotation = transform.rotation;
        
        var projectile = projectileSpawn.GetComponent<Projectile>();

        projectile.OnProjectileDestroyed += ReturnProjectileToPool;
        
        shootFeedback.PlayFeedbacks(gunPos);

        projectile.transform.parent = _projectileParent.transform;
        projectile.SetDamage(projectileDamage);
        projectile.SetShotStraight(true, projectileSpeed);
    }

    private void ReturnProjectileToPool(object sender, Projectile.OnProjectileDestroyedEventArgs eventArgs) {
        DespawnProjectile(eventArgs.Projectile);
        eventArgs.Projectile.GetComponent<Projectile>().OnProjectileDestroyed -= ReturnProjectileToPool;
    }

    private void DespawnProjectile(GameObject projectile) {
        if (usePool) _objectPool.Release(projectile);
        else Destroy(projectile);
    }
}
