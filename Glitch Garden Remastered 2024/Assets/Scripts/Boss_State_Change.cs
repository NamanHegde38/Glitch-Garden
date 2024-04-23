using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Boss_State_Change : StateMachineBehaviour {

    [SerializeField] private Vector2 middlePos = new Vector2(8, 3);
    [SerializeField] private float tweenTime = 2f;
    [SerializeField] private Ease tweenEase = Ease.InOutQuad;
    
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int WindAttack = Animator.StringToHash("WindAttack");
    private static readonly int FireAttack = Animator.StringToHash("FireAttack");
    private static readonly int ThunderAttack = Animator.StringToHash("ThunderAttack");
    private static readonly int IceAttack = Animator.StringToHash("IceAttack");
    private static readonly int PoisonAttack = Animator.StringToHash("PoisonAttack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.gameObject.transform.DOMove(middlePos, tweenTime).SetEase(tweenEase);
        
        animator.ResetTrigger(Attack);
        animator.ResetTrigger(WindAttack);
        animator.ResetTrigger(FireAttack);
        animator.ResetTrigger(ThunderAttack);
        animator.ResetTrigger(IceAttack);
        animator.ResetTrigger(PoisonAttack);
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.transform.DOKill();
    }
}
