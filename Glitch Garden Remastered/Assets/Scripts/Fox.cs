using MoreMountains.Feedbacks;
using UnityEngine;

public class Fox : MonoBehaviour {
    
    private Attacker _attacker;
    private Animator _anim;
    private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");

    [SerializeField] private MMFeedbacks landFeedback;

    private bool _hasJumped;

    
    private void Start() {
        _attacker = GetComponent<Attacker>();
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var otherObject = other.gameObject;

        if (otherObject.GetComponent<Obstruction>() && !_hasJumped) {
            _anim.SetTrigger(JumpTrigger);
            _hasJumped = true;
        }
        
        else if (otherObject.GetComponent<Defender>()) {
            _attacker.Attack(otherObject);
        }
    }
    
    public void Land() {
        landFeedback.PlayFeedbacks();
        _hasJumped = false;
    }
}
