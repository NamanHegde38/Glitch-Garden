using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Boss : MonoBehaviour {
    
    [BoxGroup("Feedbacks")]
    [SerializeField] private MMFeedbacks spawnFeedback;

    [BoxGroup("Settings")] [PropertyRange(1, 5)]
    [SerializeField] private int numberOfStages = 5;
    [BoxGroup("Settings")]
    [SerializeField] private LayerMask defenderLayer;

    [BoxGroup("Wind Attack")]
    [SerializeField] private Vector2 windAttackAreaSize = new Vector2(15, 3);
    
    [BoxGroup("Fire Attack")]
    [SerializeField] private Transform fireAttackPos;
    [BoxGroup("Fire Attack")]
    [SerializeField] private Vector2 fireAttackSize;

    [BoxGroup("Thunder Attack")]
    [SerializeField] private GameObject thunderProjectile;
    [BoxGroup("Thunder Attack")]
    [SerializeField] private Vector2 thunderAttackArea = new Vector2(5, 5);

    [BoxGroup("Ice Attack")]
    [SerializeField] private GameObject iceProjectile;
    [BoxGroup("Ice Attack")]
    [SerializeField] private float iceAttackArea = 6f;

    [BoxGroup("Poison Attack")]
    [SerializeField] private GameObject poisonProjectile;
    [BoxGroup("Poison Attack")]
    [SerializeField] private GameObject poisonMuzzle;
    [BoxGroup("Poison Attack")]
    [SerializeField] private int poisonDamage;
    [BoxGroup("Poison Attack")]
    [SerializeField] private int poisonTicks = 5;
    [BoxGroup("Poison Attack")]
    [SerializeField] private float poisonProjectileSpeed;
    [BoxGroup("Poison Attack")]
    [SerializeField] private float poisonProjectileArea;
    [BoxGroup("Poison Attack")]
    [SerializeField] private Transform poisonGunPos;
    
    [BoxGroup("Mega Laser")]
    [SerializeField] private Vector2 laserSize = new(10, 0.5f);
    
    private GameObject _projectileParent;
    private GameObject _defenderParent;
    

    private Health _healthComponent;
    private int _maxHealth;
    private int _health;

    private int _currentStage = 0;
    private float _stageThreshold;
    private float _percentToDecreaseThreshold;

    private Vector2 _transformPosition;
    private Vector2 _fireAttackPosition;

    private Animator _animator;
    private static readonly int StateChange = Animator.StringToHash("StateChange");

    private void Start() {
        _healthComponent = GetComponent<Health>();
        _maxHealth = _healthComponent.GetHealth();
        _animator = GetComponent<Animator>();

        _projectileParent = GameObject.Find("Projectiles");
        _defenderParent = GameObject.Find("Defenders");
        
        _percentToDecreaseThreshold = 1 / ((float)numberOfStages + 1);
        _stageThreshold = GetHealthThreshold(1 - _percentToDecreaseThreshold);
    }

    private void Update() {
        ConsoleProDebug.Watch(_stageThreshold.ToString(), "Stage Threshold");
        if (_healthComponent.GetHealth() > _stageThreshold) return;
        _animator.SetTrigger(StateChange);
        _currentStage++;
        if (_currentStage < numberOfStages) {
            _stageThreshold -= GetHealthThreshold(_percentToDecreaseThreshold);
        }
    }

    private float GetHealthThreshold(float percent) {
        var healthPercent = percent * _maxHealth;
        return healthPercent;
    }
    
    public void PlaySpawnFeedback() {
        spawnFeedback.PlayFeedbacks();
    }

    public void WindAttack() {
        _transformPosition = transform.position;
        var pointA = new Vector2(_transformPosition.x - windAttackAreaSize.x, (_transformPosition.y - (windAttackAreaSize.y / 2)) + 0.1f);
        var pointB = new Vector2(_transformPosition.x, (_transformPosition.y + (windAttackAreaSize.y / 2)) - 0.1f);
        var defendersToPush = Physics2D.OverlapAreaAll(pointA, pointB, defenderLayer);
        foreach (var defender in defendersToPush) {
            defender.GetComponent<Defender>().MoveDefenderX(1);
        }
    }
    
    public void FireAttack(int damage) {
        _fireAttackPosition = fireAttackPos.position;
        var pointA = new Vector2(_fireAttackPosition.x - (fireAttackSize.x / 2) + 0.1f, _fireAttackPosition.y - (fireAttackSize.y / 2) + 0.1f);
        var pointB = new Vector2(_fireAttackPosition.x + (fireAttackSize.x / 2) - 0.1f, _fireAttackPosition.y + (fireAttackSize.y / 2) - 0.1f);
        var defendersToAttack = Physics2D.OverlapAreaAll(pointA, pointB, defenderLayer);
        foreach (var defender in defendersToAttack) {
            defender.GetComponent<Health>().DealDamage(damage);
        }
    }

    public void ThunderAttack(int damage) {
        var randomTargetRaw = new Vector2(Random.Range(1, thunderAttackArea.x), Random.Range(1, thunderAttackArea.y));
        var randomTarget = SnapToGrid(randomTargetRaw);

        var projectile = Instantiate(thunderProjectile, randomTarget, Quaternion.identity);
        projectile.GetComponent<ThunderProjectile>().SetDamage(damage);
    }

    public void IceAttack() {
        var randomTargetRaw = new Vector2(Random.Range(1, iceAttackArea), 0);
        var randomTarget = SnapToGrid(randomTargetRaw);
        
        Instantiate(iceProjectile, randomTarget, Quaternion.identity);
    }

    public void PoisonAttack() {
        var position = poisonGunPos.position;
        var projectileInstance = Instantiate(poisonProjectile, position, Quaternion.identity);
        var projectile = projectileInstance.GetComponent<BossPoisonProjectile>();

        Instantiate(poisonMuzzle, position, Quaternion.identity);
        
        projectile.transform.parent = _projectileParent.transform;
        projectile.SetDamage(poisonDamage);
        projectile.SetProjectileSpeed(poisonProjectileSpeed);
        projectile.SetDamageArea(poisonProjectileArea);
        projectile.SetTicks(poisonTicks);
        if (_defenderParent.transform.childCount > 0) {
            projectile.GetTarget(GetRandomDefenderPos());
        }

        Destroy(projectileInstance, 10f);
    }

    public void MegaLaser(int damage) {
        var position = transform.position;
        var pointA = new Vector2(position.x - laserSize.x + 0.1f, position.y - laserSize.y + 0.1f);
        var pointB = new Vector2(position.x - 0.1f, position.y + laserSize.y - 0.1f);
        var defendersToAttack = Physics2D.OverlapAreaAll(pointA, pointB, defenderLayer);
        foreach (var defender in defendersToAttack) {
            defender.GetComponent<Health>().DealDamage(damage);
        }
    }

    public void FinishLevel() {
        FindObjectOfType<LevelController>().WinLevel();
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
}
