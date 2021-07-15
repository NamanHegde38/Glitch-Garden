using UnityEngine;

public class Bat : MonoBehaviour {
    
    private Attacker _attacker;

    private void Start() {
        _attacker = GetComponent<Attacker>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var otherObject = other.gameObject;

        if (otherObject.GetComponent<Defender>()) {
            _attacker.Attack(otherObject);
        }
    }
}
