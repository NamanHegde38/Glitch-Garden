using UnityEngine;

public class Lobber : MonoBehaviour {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject gun;
    [SerializeField] private int projectileDamage = 25;
    
    [SerializeField] private LayerMask attackerLayer;

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
        //return _myLaneSpawner.transform.childCount > 0;
        
        return _myLaneSpawner.transform.childCount > 0 ? true : Physics2D.Raycast(gun.transform.position, Vector2.right, 8f, attackerLayer);
    }

    public void Fire() {
        if (_myLaneSpawner.transform.childCount <= 0) return;
        
        var closestEnemy = _myLaneSpawner.transform.GetChild(0);
        var projectile = Instantiate(projectilePrefab, gun.transform.position, transform.rotation);
        
        projectile.transform.parent = _projectileParent.transform;
        projectile.GetComponent<Projectile>().SetDamage(projectileDamage);
        projectile.GetComponent<Rigidbody2D>().velocity = CalculateVelocity(closestEnemy.transform);
        Destroy(projectile, 5);
    }

    private Vector3 CalculateVelocity(Transform target) {
        var direction = target.position - gun.transform.position;
        var height = direction.y;
        direction.y = 0;
        var distance = direction.magnitude;
        direction.y = distance;
        distance += height;
        var vel = Mathf.Sqrt(distance * Physics.gravity.magnitude);
        return vel * direction.normalized;
    }
}
