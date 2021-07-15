using UnityEngine;

public class Ghost : MonoBehaviour {
    
    private Attacker _attacker;
    private Animator _anim;
    private static readonly int PhaseTrigger = Animator.StringToHash("PhaseTrigger");
    private BoxCollider2D _collider;

    private void Start() {
        _attacker = GetComponent<Attacker>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var otherObject = other.gameObject;

        if (otherObject.GetComponent<Obstruction>()) {
            _anim.SetTrigger(PhaseTrigger);
        }
        
        else if (otherObject.GetComponent<Defender>()) {
            _attacker.Attack(otherObject);
        }
    }
}
