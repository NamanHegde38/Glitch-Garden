using System;
using UnityEngine;

public class DamageCollider : MonoBehaviour {

    private LivesDisplay _livesDisplay;
    private LevelController _levelController;

    private void Start() {
        _levelController = FindObjectOfType<LevelController>();
        _levelController.OnLevelStart += SetComponent;
    }

    private void SetComponent(object sender, EventArgs e) {
        _livesDisplay = FindObjectOfType<LivesDisplay>();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        _livesDisplay.TakeLife();
    }
}
