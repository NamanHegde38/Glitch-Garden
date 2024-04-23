using DG.Tweening;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour {

    [SerializeField] [Range(0, 1)] private float moveChance = 0.125f;
    [SerializeField] [Range(0, 1)] private float standardAttackChance = 0.125f;
    [SerializeField] [Range(0, 1)] private float windAttackChance = 0.125f;
    [SerializeField] [Range(0, 1)] private float fireAttackChance = 0.125f;
    [SerializeField] [Range(0, 1)] private float thunderAttackChance = 0.125f;
    [SerializeField] [Range(0, 1)] private float iceAttackChance = 0.125f;
    [SerializeField] [Range(0, 1)] private float poisonAttackChance = 0.125f;
    [SerializeField] [Range(0, 1)] private float megaLaserChance = 0.125f;
    
    private static readonly int Move = Animator.StringToHash("Move");

    private const int MAXTimer = 1;
    private float _timer = 1;
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    private GameObject _defenderParent;
    private static readonly int WindAttack = Animator.StringToHash("WindAttack");
    private static readonly int FireAttack = Animator.StringToHash("FireAttack");
    private static readonly int ThunderAttack = Animator.StringToHash("ThunderAttack");
    private static readonly int IceAttack = Animator.StringToHash("IceAttack");
    private static readonly int PoisonAttack = Animator.StringToHash("PoisonAttack");
    private static readonly int MegaLaser = Animator.StringToHash("MegaLaser");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _defenderParent = GameObject.Find("Defenders");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_timer <= 0) {
            var rng = Random.Range(0f, 1f);
            if (rng < moveChance) {
                animator.SetTrigger(Move);
                _timer = MAXTimer;
            }
            
            else if (rng < moveChance + windAttackChance) {
                if (_defenderParent.transform.childCount > 0) {
                    animator.SetTrigger(WindAttack);
                    _timer = MAXTimer;
                }
            }
            
            else if (rng < moveChance + windAttackChance + fireAttackChance) {
                if (_defenderParent.transform.childCount > 0) {
                    animator.SetTrigger(FireAttack);
                    _timer = MAXTimer;
                }
            }
            
            else if (rng < moveChance + windAttackChance + fireAttackChance + thunderAttackChance) {
                if (_defenderParent.transform.childCount > 0) {
                    animator.SetTrigger(ThunderAttack);
                    _timer = MAXTimer;
                }
            }
            
            else if (rng < moveChance + windAttackChance + fireAttackChance + thunderAttackChance + iceAttackChance) {
                if (_defenderParent.transform.childCount > 0) {
                    animator.SetTrigger(IceAttack);
                    _timer = MAXTimer;
                }
            }
            
            else if (rng < moveChance + windAttackChance + fireAttackChance + thunderAttackChance + iceAttackChance + poisonAttackChance) {
                if (_defenderParent.transform.childCount > 0) {
                    animator.SetTrigger(PoisonAttack);
                    _timer = MAXTimer;
                }
            }
            
            else if (rng < moveChance + windAttackChance + fireAttackChance + thunderAttackChance + iceAttackChance + poisonAttackChance + megaLaserChance) {
                if (_defenderParent.transform.childCount > 0) {
                    animator.SetTrigger(MegaLaser);
                    _timer = MAXTimer;
                }
            }
            
            else if (rng < moveChance + windAttackChance + fireAttackChance + thunderAttackChance + iceAttackChance + poisonAttackChance + megaLaserChance + standardAttackChance) {
                if (_defenderParent.transform.childCount > 0) {
                    animator.SetTrigger(Attack);
                    _timer = MAXTimer;
                }
            }
        }
        else {
            _timer -= Time.deltaTime;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.transform.DOKill();
        _timer = MAXTimer;
    }
}
