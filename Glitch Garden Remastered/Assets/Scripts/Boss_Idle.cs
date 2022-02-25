using System.Collections;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour {

    [SerializeField] [Range(0, 1)] private float moveChance = 0.5f;
    private static readonly int Move = Animator.StringToHash("Move");

    private const int MAXTimer = 1;
    private float _timer = 1;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_timer <= 0) {
            if (Random.Range(0f, 1f) < moveChance) {
                animator.SetTrigger(Move);
            }
            _timer = MAXTimer;
        }
        else {
            _timer -= Time.deltaTime;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }
}
