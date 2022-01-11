using UnityEngine;

public class Attacker : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private Transform attackPos;

    [Header("Particles")]
    [SerializeField] private GameObject woodParticles;
    [SerializeField] private GameObject rockParticles;
    [SerializeField] private GameObject stoneParticles;
    [SerializeField] private GameObject metalParticles;
    [SerializeField] private GameObject organicParticles;

    [SerializeField] private float particlesLifetime = 0.3f;

    [SerializeField] private bool hasSpawnAnimation;

    private float _currentSpeed;
    private GameObject _currentTarget;
    private Animator _anim;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private LevelController _levelController;

    private int _difficulty = 1;
    
    private void Awake() {
        _levelController = FindObjectOfType<LevelController>();
        
        if (!_levelController) return;
        _levelController.AttackerSpawned();
    }

    private void Start() {
        _difficulty = PlayerPrefsController.GetDifficulty();
        _anim = GetComponent<Animator>();
    }

    private void Update() {
        transform.Translate(Vector2.left * (_currentSpeed * Time.deltaTime));
        UpdateAnimationState();
    }

    private void UpdateAnimationState() {
        if (!_currentTarget) {
            _anim.SetBool(IsAttacking, false);
        }
    }

    public void SetMovementSpeed(float speed) {
        _currentSpeed = speed;
    }

    public void Attack(GameObject target) {
        _anim.SetBool(IsAttacking, true);
        _currentTarget = target;
        
    }

    public void StrikeCurrentTarget(float damage) {
        if (!_currentTarget) return;
        
        var health = _currentTarget.GetComponent<Health>();
        var material = _currentTarget.GetComponent<Defender>().GetDefenderMaterial();

        switch (material) {
            case DefenderMaterial.Wood:
                var spawnedWoodParticles = Instantiate(woodParticles, attackPos.position, Quaternion.identity);
                Destroy(spawnedWoodParticles, particlesLifetime);
                break;
            case DefenderMaterial.Rock:
                var spawnedRockParticles = Instantiate(rockParticles, attackPos.position, Quaternion.identity);
                Destroy(spawnedRockParticles, particlesLifetime);
                break;
            case DefenderMaterial.Stone:
                var spawnedStoneParticles = Instantiate(stoneParticles, attackPos.position, Quaternion.identity);
                Destroy(spawnedStoneParticles, particlesLifetime);
                break;
            case DefenderMaterial.Metal:
                var spawnedMetalParticles = Instantiate(metalParticles, attackPos.position, Quaternion.identity);
                Destroy(spawnedMetalParticles, particlesLifetime);
                break;
            case DefenderMaterial.Organic:
                var spawnedOrganicParticles = Instantiate(organicParticles, attackPos.position, Quaternion.identity);
                Destroy(spawnedOrganicParticles, particlesLifetime);
                break;
        }
        
        if (!health) return;
        
        switch (_difficulty) {
            case 1:
                damage *= 1f;
                break;
            case 2:
                damage *= 1.25f;
                break;
            case 3:
                damage *= 1.5f;
                break;
        }
        health.DealDamage(Mathf.RoundToInt(damage));
    }

    public bool GetIfHasSpawnAnimation() {
        return hasSpawnAnimation;
    }

    private void OnDestroy() {
        if (!_levelController) return;
        _levelController.AttackerKilled();
    }
    
    
    
    
    
    
}
