using UnityEngine;

public class Obstruction : MonoBehaviour {
    private void OnTriggerStay2D(Collider2D other) {
        var attacker = other.GetComponent<Attacker>();

        if (attacker) {
            
        }
    }
}
