using DG.Tweening;
using UnityEngine;

public class Boss_Attack : StateMachineBehaviour {

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject muzzlePrefab;
    [SerializeField] private int projectileDamage = 25;
    [SerializeField] private float projectileSpeed = 1f;
    [SerializeField] private float projectileArea = 1f;
    [SerializeField] private int maxNumberOfTimesToAttack = 4;
    [SerializeField] private float timeBetweenAttacks = 2f;

    private int _numberOfTimesToAttack;
    private int _timesAttacked = 0;
    private GameObject _projectileParent;
    private GameObject _defenderParent;
    private float _timer;
    private static readonly int StopAttack = Animator.StringToHash("StopAttack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _timer = 0;
        _timesAttacked = 0;
        _numberOfTimesToAttack = Random.Range(1, maxNumberOfTimesToAttack + 1);
        _projectileParent = GameObject.Find("Projectiles");
        _defenderParent = GameObject.Find("Defenders");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_timer <= 0) {
            if (_timesAttacked < _numberOfTimesToAttack) {
                var position = animator.gameObject.transform.position;
                var projectileInstance = Instantiate(projectilePrefab, position, Quaternion.identity);
                var projectile = projectileInstance.GetComponent<BossProjectile>();

                Instantiate(muzzlePrefab, position, Quaternion.identity);
            
                projectile.transform.parent = _projectileParent.transform;
                projectile.SetDamage(projectileDamage);
                projectile.SetProjectileSpeed(projectileSpeed);
                projectile.SetDamageArea(projectileArea);
                projectile.GetTarget(GetRandomDefenderPos());
            
                Destroy(projectileInstance, 10f);
                _timesAttacked++;
                
            }
            else {
                animator.SetTrigger(StopAttack);
            }
            _timer = timeBetweenAttacks;
        }
        else {
            _timer -= Time.deltaTime;
        }
    }

    private Vector2 GetRandomDefenderPos() {
        var randomChild = Random.Range(0, _defenderParent.transform.childCount);
        var randomDefender = _defenderParent.transform.GetChild(randomChild);
        return SnapToGrid(randomDefender.position);
    }
    
    private static Vector2 SnapToGrid(Vector2 rawWorldPos) {
        var newX = Mathf.RoundToInt(rawWorldPos.x);
        var newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector2(newX, newY);
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.transform.DOKill();
    }
}
