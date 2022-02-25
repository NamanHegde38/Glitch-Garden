using UnityEngine;
using DG.Tweening;

public class Boss_Move : StateMachineBehaviour {

    [SerializeField] private int minX = 7;
    [SerializeField] private int maxX = 9;
    [SerializeField] private int minY = 1;
    [SerializeField] private int maxY = 5;

    [SerializeField] private float tweenTime = 2f;
    [SerializeField] private Ease tweenEase = Ease.InOutQuad;
    private static readonly int Stop = Animator.StringToHash("Stop");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var randomPos = new Vector2(Random.Range(minX, maxX + 1), Random.Range(minY, maxY + 1)); 
        animator.gameObject.transform.DOMove(randomPos, tweenTime).SetEase(tweenEase).OnComplete(() => animator.SetTrigger(Stop));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }
}
