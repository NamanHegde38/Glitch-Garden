using MoreMountains.Feedbacks;
using UnityEngine;

public class Lizard : MonoBehaviour {

    private Attacker _attacker;

    [SerializeField] private MMFeedbacks landFeedback;

    private void Start() {
        _attacker = GetComponent<Attacker>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var otherObject = other.gameObject;

        if (otherObject.GetComponent<Defender>()) {
            _attacker.Attack(otherObject);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        var otherObject = other.gameObject;
        if (otherObject.GetComponent<Defender>()) {
            _attacker.StopAttack();
        }
    }

    public void Land() {
        landFeedback.PlayFeedbacks();
    }
}
