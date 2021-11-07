using UnityEngine;

public class Shooter : MonoBehaviour {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject gun;
    [SerializeField] private float projectileSpeed = 1f;
    [SerializeField] private int projectileDamage = 25;

    private AttackerSpawner _myLaneSpawner;
    private Animator _animator;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private GameObject _projectileParent;

    private const string Projectiles = "Projectiles";

    private void Start() {
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
        return _myLaneSpawner.transform.childCount > 0;
    }
    
    public void Fire() {
        var projectileSpawn = Instantiate(projectilePrefab, gun.transform.position, transform.rotation);
        var projectile = projectileSpawn.GetComponent<Projectile>();

        projectile.transform.parent = _projectileParent.transform;
        projectile.SetDamage(projectileDamage);
        projectile.SetShotStraight(true, projectileSpeed);
        
        Destroy(projectile, 5f);
    }
}
