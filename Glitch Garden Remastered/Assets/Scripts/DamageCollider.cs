using System;
using UnityEngine;

public class DamageCollider : MonoBehaviour {

    private LivesDisplay _livesDisplay;

    private void Start() {
        _livesDisplay = FindObjectOfType<LivesDisplay>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        _livesDisplay.TakeLife();
    }
}
