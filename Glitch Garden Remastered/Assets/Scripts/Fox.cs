using UnityEngine;

public class Fox : MonoBehaviour {
    
    private Attacker _attacker;
    private Animator _anim;
    private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");

    private void Start() {
        _attacker = GetComponent<Attacker>();
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var otherObject = other.gameObject;

        if (otherObject.GetComponent<Obstruction>()) {
            _anim.SetTrigger(JumpTrigger);
        }
        
        else if (otherObject.GetComponent<Defender>()) {
            _attacker.Attack(otherObject);
        }
    }
}
