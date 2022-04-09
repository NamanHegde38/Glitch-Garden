using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Boss_State_Change : StateMachineBehaviour {

    [SerializeField] private Vector2 middlePos = new Vector2(8, 3);
    [SerializeField] private float tweenTime = 2f;
    [SerializeField] private Ease tweenEase = Ease.InOutQuad;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.gameObject.transform.DOMove(middlePos, tweenTime).SetEase(tweenEase);
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.transform.DOKill();
    }
}
